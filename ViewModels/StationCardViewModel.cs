using System;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 개별 스테이션 카드(ST1~ST4) 에 대한 ViewModel.
    /// StationCardView.xaml 에 DataContext 로 바인딩됩니다.
    ///
    /// VisionPro 9.x 통합:
    ///   - CameraService(ICognexCameraService) 를 주입받아 카메라 연결·획득·검사를 수행합니다.
    ///   - ImageAcquired 이벤트로 View(StationDisplayView.xaml.cs) 에 이미지를 전달합니다.
    ///   - AttachDisplay() 로 CogDisplay 를 카메라 서비스에 등록합니다.
    /// </summary>
    public class StationCardViewModel : ViewModelBase, IDisposable
    {
        private int _stationId;
        private string _stationName = string.Empty;
        private bool _isLive;
        private bool _isFitMode;
        private bool _isOneToOne;
        private StationResultState _resultState;
        private double _pressureValue;
        private double _threshold;
        private bool _isMonitoring;
        private bool _disposed;

        /// <summary>
        /// VisionPro 카메라 서비스. StationGroupViewModel 에서 주입됩니다.
        /// null 이면 카메라 미사용(서비스 없음) 상태입니다.
        /// </summary>
        public ICognexCameraService? CameraService { get; set; }

        // ── 이벤트 ───────────────────────────────────────────────────────

        /// <summary>이미지가 획득될 때 View(StationDisplayView.xaml.cs) 에 전달되는 이벤트.</summary>
        public event Action<ICogImage>? ImageAcquired;

        // ── 기본 식별 속성 ──────────────────────────────────────────

        public int StationId
        {
            get => _stationId;
            set => SetProperty(ref _stationId, value);
        }

        public string StationName
        {
            get => _stationName;
            set => SetProperty(ref _stationName, value);
        }

        // ── 표시 모드 속성 ───────────────────────────────────────────

        /// <summary>라이브 영상 표시 모드 활성 여부</summary>
        public bool IsLive
        {
            get => _isLive;
            set => SetProperty(ref _isLive, value);
        }

        /// <summary>FIT(화면 맞춤) 표시 모드 활성 여부</summary>
        public bool IsFitMode
        {
            get => _isFitMode;
            set => SetProperty(ref _isFitMode, value);
        }

        /// <summary>1:1 픽셀 표시 모드 활성 여부</summary>
        public bool IsOneToOne
        {
            get => _isOneToOne;
            set => SetProperty(ref _isOneToOne, value);
        }

        // ── 검사 결과 속성 ──────────────────────────────────────────

        /// <summary>OK / NG / Unknown 판정 결과</summary>
        public StationResultState ResultState
        {
            get => _resultState;
            set => SetProperty(ref _resultState, value, () =>
            {
                OnPropertyChanged(nameof(IsOK));
                OnPropertyChanged(nameof(IsNG));
                OnPropertyChanged(nameof(ResultText));
            });
        }

        public bool IsOK => ResultState == StationResultState.OK;
        public bool IsNG => ResultState == StationResultState.NG;

        /// <summary>결과 텍스트 표시용 ("OK" / "NG" / "---")</summary>
        public string ResultText => ResultState switch
        {
            StationResultState.OK => "OK",
            StationResultState.NG => "NG",
            _ => "---",
        };

        // ── 측정값 속성 ─────────────────────────────────────────────

        /// <summary>현재 압력 측정값 (kPa)</summary>
        public double PressureValue
        {
            get => _pressureValue;
            set => SetProperty(ref _pressureValue, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        /// <summary>누설 판단 임계값 (kPa)</summary>
        public double Threshold
        {
            get => _threshold;
            set => SetProperty(ref _threshold, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        /// <summary>측정값이 임계값을 초과하는지 여부</summary>
        public bool IsAboveThreshold => PressureValue > Threshold;

        /// <summary>현재 모니터링 동작 중 여부</summary>
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        // ── Commands ──────────────────────────────────────────────────

        /// <summary>LIVE 모드로 전환 (카메라 서비스가 있으면 StartLive() 호출)</summary>
        public RelayCommand LiveCommand { get; }

        /// <summary>1:1 픽셀 모드로 전환</summary>
        public RelayCommand OneToOneCommand { get; }

        /// <summary>FIT(화면 맞춤) 모드 토글</summary>
        public RelayCommand FitCommand { get; }

        /// <summary>단일 이미지 획득</summary>
        public RelayCommand GrabCommand { get; }

        /// <summary>VPP 검사 실행 (획득 후 바로 검사)</summary>
        public RelayCommand InspectCommand { get; }

        // ── 생성자 ────────────────────────────────────────────────────

        public StationCardViewModel()
        {
            LiveCommand = new RelayCommand(OnLive);
            OneToOneCommand = new RelayCommand(OnOneToOne);
            FitCommand = new RelayCommand(OnFit);
            GrabCommand = new RelayCommand(OnGrab);
            InspectCommand = new RelayCommand(OnInspect);
        }

        // ── 디스플레이 연결 ──────────────────────────────────────────

        /// <summary>
        /// View 에서 CogDisplay 를 전달받아 카메라 서비스에 등록합니다.
        /// </summary>
        public void AttachDisplay(CogDisplay display)
        {
            CameraService?.SetDisplay(display);

            // 카메라 서비스의 ImageAcquired 이벤트를 이 VM 의 이벤트로 전달
            if (CameraService != null)
                CameraService.ImageAcquired += OnCameraImageAcquired;
        }

        // ── Command Handlers ──────────────────────────────────────────

        private void OnLive()
        {
            IsLive = true;
            IsFitMode = false;
            IsOneToOne = false;

            if (CameraService?.IsConnected == true)
                CameraService.StartLive();
        }

        private void OnOneToOne()
        {
            IsOneToOne = true;
            IsLive = false;
            IsFitMode = false;

            CameraService?.StopLive();
        }

        private void OnFit()
        {
            IsFitMode = true;
            IsLive = false;
            IsOneToOne = false;

            CameraService?.StopLive();
        }

        private void OnGrab()
        {
            if (CameraService?.IsConnected != true) return;

            try
            {
                CameraService.Grab();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"ST{StationId} 이미지 획득 실패: {ex.Message}",
                    "카메라 오류",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }

        private void OnInspect()
        {
            if (CameraService?.IsConnected != true) return;
            if (!CameraService.IsVppLoaded)
            {
                System.Windows.MessageBox.Show(
                    $"ST{StationId}: VPP 파일이 로드되지 않았습니다.",
                    "검사 오류",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                ICogImage image = CameraService.Grab();
                InspectionResult result = CameraService.RunInspection(image);
                ResultState = result.Passed ? StationResultState.OK : StationResultState.NG;
            }
            catch (Exception ex)
            {
                ResultState = StationResultState.NG;
                System.Windows.MessageBox.Show(
                    $"ST{StationId} 검사 실패: {ex.Message}",
                    "검사 오류",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }

        private void OnCameraImageAcquired(ICogImage image)
        {
            ImageAcquired?.Invoke(image);
        }

        // ── IDisposable ───────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (CameraService != null)
            {
                CameraService.ImageAcquired -= OnCameraImageAcquired;
                CameraService.Dispose();
                CameraService = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
