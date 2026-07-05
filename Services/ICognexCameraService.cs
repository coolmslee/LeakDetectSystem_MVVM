using System;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    /// <summary>
    /// VisionPro 9.x 카메라 1대에 대한 연결·획득·검사 서비스 인터페이스.
    /// 각 스테이션(CAM1~4)마다 인스턴스를 생성하여 사용합니다.
    /// </summary>
    public interface ICognexCameraService : IDisposable
    {
        // ── 상태 ──────────────────────────────────────────────────────────────

        /// <summary>카메라 연결 여부</summary>
        bool IsConnected { get; }

        /// <summary>VPP 파일이 로드되어 있는지 여부</summary>
        bool IsVppLoaded { get; }

        // ── 이벤트 ───────────────────────────────────────────────────────────

        /// <summary>이미지가 획득될 때마다 발생합니다 (LIVE 모드 포함).</summary>
        event Action<ICogImage>? ImageAcquired;

        /// <summary>연결 상태가 변경될 때 발생합니다.</summary>
        event Action<bool>? ConnectionChanged;

        // ── 연결 / 해제 ──────────────────────────────────────────────────────

        /// <summary>
        /// 지정 카메라 설정으로 연결합니다.
        /// </summary>
        /// <param name="config">IP·ExposureTime·Gain 등 카메라 설정</param>
        /// <exception cref="InvalidOperationException">이미 연결된 경우</exception>
        /// <exception cref="Exception">카메라를 찾지 못하거나 초기화 실패</exception>
        void Connect(CameraConfig config);

        /// <summary>카메라 연결을 해제합니다.</summary>
        void Disconnect();

        // ── 획득 ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 단일 이미지를 획득합니다.
        /// </summary>
        /// <returns>획득된 <see cref="ICogImage"/></returns>
        ICogImage Grab();

        /// <summary>
        /// LIVE 이미지 획득을 시작합니다.
        /// 획득된 이미지는 <see cref="ImageAcquired"/> 이벤트로 전달됩니다.
        /// </summary>
        void StartLive();

        /// <summary>LIVE 이미지 획득을 중지합니다.</summary>
        void StopLive();

        // ── VPP 검사 ─────────────────────────────────────────────────────────

        /// <summary>
        /// VisionPro VPP 파일을 로드합니다.
        /// </summary>
        /// <param name="vppFilePath">VPP 파일 전체 경로</param>
        void LoadVpp(string vppFilePath);

        /// <summary>
        /// 지정 이미지에 대해 로드된 VPP Tool을 실행하고 결과를 반환합니다.
        /// </summary>
        /// <param name="image">검사할 이미지</param>
        /// <returns>검사 결과</returns>
        InspectionResult RunInspection(ICogImage image);

        // ── 디스플레이 ────────────────────────────────────────────────────────

        /// <summary>
        /// CogDisplay 인스턴스를 등록합니다.
        /// 획득된 이미지가 자동으로 이 디스플레이에 표시됩니다.
        /// </summary>
        void SetDisplay(CogDisplay display);
    }
}
