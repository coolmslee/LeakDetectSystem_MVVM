using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ToolBlock;
using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    /// <summary>
    /// VisionPro 9.x API를 사용하여 GigE 카메라 1대를 제어하는 서비스 구현체.
    ///
    /// VisionPro 9.x GigE 카메라 연결 흐름:
    ///   1. CogFrameGrabberGigE.CreateAll() → 연결 가능한 모든 프레임 그래버 열거
    ///   2. IP 주소로 대상 그래버 탐색
    ///   3. ICogAcqFifo 생성 후 ExposureTime / Gain 설정
    ///   4. ICogAcqFifo.Acquire() 로 이미지 획득
    ///   5. CogToolBlock 으로 VPP 검사 실행
    /// </summary>
    public sealed class CognexCameraService : ICognexCameraService
    {
        // ── 필드 ─────────────────────────────────────────────────────────────

        private ICogFrameGrabber? _frameGrabber;
        private ICogAcqFifo? _acqFifo;
        private CogToolBlock? _toolBlock;
        private CogDisplay? _display;

        private Thread? _liveThread;
        private volatile bool _liveRunning;
        private int _stationIndex;
        private CameraConfig? _config;
        private bool _disposed;

        // ── 이벤트 ───────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public event Action<ICogImage>? ImageAcquired;

        /// <inheritdoc/>
        public event Action<bool>? ConnectionChanged;

        // ── 상태 ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public bool IsConnected => _acqFifo != null;

        /// <inheritdoc/>
        public bool IsVppLoaded => _toolBlock != null;

        // ── 연결 / 해제 ──────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void Connect(CameraConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (IsConnected)
                throw new InvalidOperationException($"CAM{config.Index}은 이미 연결되어 있습니다. 먼저 Disconnect()를 호출하십시오.");

            _config = config;
            _stationIndex = config.Index;

            // ── 1. 사용 가능한 GigE 프레임 그래버 열거 ──
            List<ICogFrameGrabber> grabbers = CogFrameGrabberGigE.CreateAll();
            if (grabbers == null || grabbers.Count == 0)
                throw new Exception($"CAM{config.Index}: 연결 가능한 GigE 카메라가 없습니다. VisionPro GigE 드라이버 설치를 확인하십시오.");

            // ── 2. IP 주소로 대상 그래버 탐색 ──
            _frameGrabber = FindGrabberByIp(grabbers, config.Ip);
            if (_frameGrabber == null)
                throw new Exception($"CAM{config.Index}: IP {config.Ip} 에 해당하는 카메라를 찾을 수 없습니다.");

            // ── 3. Acquisition FIFO 생성 ──
            // 사용 가능한 첫 번째 비디오 포맷으로 FIFO 생성
            string videoFormat = _frameGrabber.AvailableVideoFormats.Length > 0
                ? _frameGrabber.AvailableVideoFormats[0]
                : config.VideoFormat;

            _acqFifo = _frameGrabber.CreateAcqFifo(
                videoFormat,
                CogAcqFifoPixelFormatConstants.Format8Grey,
                0,
                true);

            // ── 4. 카메라 파라미터 설정 ──
            ApplyCameraParameters(config);

            ConnectionChanged?.Invoke(true);
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            StopLive();

            if (_acqFifo != null)
            {
                try { _acqFifo.Flush(); } catch { /* 무시 */ }
                _acqFifo = null;
            }

            _frameGrabber = null;
            ConnectionChanged?.Invoke(false);
        }

        // ── 획득 ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public ICogImage Grab()
        {
            EnsureConnected();

            int triggerBit;
            bool completed;

            ICogImage image = _acqFifo!.Acquire(out triggerBit, out completed);
            if (image == null)
                throw new Exception($"CAM{_stationIndex}: 이미지 획득에 실패했습니다.");

            NotifyImageAcquired(image);
            return image;
        }

        /// <inheritdoc/>
        public void StartLive()
        {
            if (_liveRunning) return;
            EnsureConnected();

            _liveRunning = true;
            _liveThread = new Thread(LiveLoop)
            {
                Name = $"CognexLive_CAM{_stationIndex}",
                IsBackground = true,
            };
            _liveThread.Start();
        }

        /// <inheritdoc/>
        public void StopLive()
        {
            _liveRunning = false;
            _liveThread?.Join(2000);
            _liveThread = null;
        }

        // ── VPP 검사 ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void LoadVpp(string vppFilePath)
        {
            if (string.IsNullOrWhiteSpace(vppFilePath))
                throw new ArgumentNullException(nameof(vppFilePath));

            var loaded = CogSerializer.LoadObjectFromFile(vppFilePath) as CogToolBlock;
            if (loaded == null)
                throw new InvalidOperationException($"VPP 파일을 CogToolBlock으로 로드하지 못했습니다: {vppFilePath}");

            _toolBlock = loaded;
        }

        /// <inheritdoc/>
        public InspectionResult RunInspection(ICogImage image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (_toolBlock == null)
                return InspectionResult.Ng(_stationIndex, "VPP 파일이 로드되지 않았습니다. LoadVpp()를 먼저 호출하십시오.");

            try
            {
                // 입력 이미지 설정 – VPP의 InputImage 터미널 이름과 일치해야 합니다
                if (_toolBlock.Inputs.Contains("InputImage"))
                    _toolBlock.Inputs["InputImage"].Value = image;

                _toolBlock.Run();

                // 검사 결과 읽기 – VPP의 출력 터미널 이름과 일치해야 합니다
                bool passed = false;
                if (_toolBlock.Outputs.Contains("PassFail"))
                    passed = Convert.ToBoolean(_toolBlock.Outputs["PassFail"].Value);

                return passed
                    ? InspectionResult.Ok(_stationIndex)
                    : InspectionResult.Ng(_stationIndex);
            }
            catch (Exception ex)
            {
                return InspectionResult.Ng(_stationIndex, ex.Message);
            }
        }

        // ── 디스플레이 ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void SetDisplay(CogDisplay display)
        {
            _display = display;
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Disconnect();
            _toolBlock = null;
            _display = null;
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        /// <summary>IP 주소로 프레임 그래버를 탐색합니다.</summary>
        private static ICogFrameGrabber? FindGrabberByIp(List<ICogFrameGrabber> grabbers, string ipAddress)
        {
            foreach (ICogFrameGrabber grabber in grabbers)
            {
                try
                {
                    // VisionPro 9.x GigE 그래버는 Attributes 딕셔너리에 IP_Address를 포함합니다.
                    object? attr = grabber.Attributes["IP_Address"];
                    if (attr != null && string.Equals(attr.ToString(), ipAddress, StringComparison.OrdinalIgnoreCase))
                        return grabber;
                }
                catch
                {
                    // Attributes 접근 실패 시 다음 그래버로 진행
                }
            }

            return null;
        }

        /// <summary>카메라 파라미터(노출·게인)를 획득 FIFO에 적용합니다.</summary>
        private void ApplyCameraParameters(CameraConfig config)
        {
            if (_acqFifo == null) return;

            // 노출 시간 설정 (마이크로초 단위)
            ICogAcqExposureParams? expParams = _acqFifo.OwnedExposureParams;
            if (expParams != null)
            {
                try { expParams.Exposure = config.ExposureTime; } catch { /* 카메라가 지원하지 않으면 무시 */ }
            }

            // 게인 설정
            ICogAcqGainParams? gainParams = _acqFifo.OwnedGainParams;
            if (gainParams != null)
            {
                try { gainParams.Gain = config.Gain; } catch { /* 카메라가 지원하지 않으면 무시 */ }
            }
        }

        /// <summary>이미지 획득 알림 및 CogDisplay 업데이트.</summary>
        private void NotifyImageAcquired(ICogImage image)
        {
            // CogDisplay 업데이트는 UI 스레드에서 수행
            if (_display != null)
            {
                try
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        _display.Image = image;
                    });
                }
                catch { /* UI 스레드 접근 실패 시 무시 */ }
            }

            ImageAcquired?.Invoke(image);
        }

        /// <summary>LIVE 모드 백그라운드 루프.</summary>
        private void LiveLoop()
        {
            while (_liveRunning && IsConnected)
            {
                try
                {
                    int triggerBit;
                    bool completed;
                    ICogImage image = _acqFifo!.Acquire(out triggerBit, out completed);
                    if (image != null)
                        NotifyImageAcquired(image);
                }
                catch
                {
                    // 획득 오류 시 잠시 대기 후 재시도
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>연결 상태를 확인하고 미연결 시 예외를 발생시킵니다.</summary>
        private void EnsureConnected()
        {
            if (!IsConnected)
                throw new InvalidOperationException($"CAM{_stationIndex}: 카메라가 연결되어 있지 않습니다. Connect()를 먼저 호출하십시오.");
        }
    }
}
