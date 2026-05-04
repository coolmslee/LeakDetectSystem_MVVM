using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 연결상태/요청신호 패널 ViewModel.
    /// PLC, 카메라, IMQS, RFID 연결 상태와 요청/결과 신호를 표시하고
    /// 테스트용 토글 Command를 제공합니다.
    ///
    /// 원본(LeakDetectSystem) 기준:
    ///   MainWindow.xaml 의 '연결상태/요청신호/통신 상태' Ellipse 영역에 대응합니다.
    /// </summary>
    public class ConnectionStatePanelViewModel : ViewModelBase
    {
        // ───────────────── 연결 상태 속성 ─────────────────

        private bool _isPlcConnected;
        private bool _isCameraConnected;
        private bool _isImqsConnected;
        private bool _isRfidConnected;

        /// <summary>PLC 연결 상태</summary>
        public bool IsPlcConnected
        {
            get => _isPlcConnected;
            set => SetProperty(ref _isPlcConnected, value);
        }

        /// <summary>카메라 연결 상태</summary>
        public bool IsCameraConnected
        {
            get => _isCameraConnected;
            set => SetProperty(ref _isCameraConnected, value);
        }

        /// <summary>IMQS 연결 상태</summary>
        public bool IsImqsConnected
        {
            get => _isImqsConnected;
            set => SetProperty(ref _isImqsConnected, value);
        }

        /// <summary>RFID 연결 상태</summary>
        public bool IsRfidConnected
        {
            get => _isRfidConnected;
            set => SetProperty(ref _isRfidConnected, value);
        }

        // ───────────────── 요청/결과 신호 속성 ─────────────────

        private bool _isRequestSignalActive;
        private bool _isResultSignalActive;
        private bool _isCommunicationActive;

        /// <summary>요청 신호 활성 여부</summary>
        public bool IsRequestSignalActive
        {
            get => _isRequestSignalActive;
            set => SetProperty(ref _isRequestSignalActive, value);
        }

        /// <summary>결과 신호 활성 여부</summary>
        public bool IsResultSignalActive
        {
            get => _isResultSignalActive;
            set => SetProperty(ref _isResultSignalActive, value);
        }

        /// <summary>통신 상태 활성 여부</summary>
        public bool IsCommunicationActive
        {
            get => _isCommunicationActive;
            set => SetProperty(ref _isCommunicationActive, value);
        }

        // ───────────────── Commands ─────────────────

        /// <summary>
        /// PLC 연결 상태 토글.
        /// 개발·디버그 환경에서 UI 검증에 사용합니다.
        /// 실제 운용 시에는 PLC 서비스 레이어의 연결/해제 결과로 IsPlcConnected를 갱신하고
        /// 이 Command는 제거하거나 비활성화합니다.
        /// </summary>
        public RelayCommand TogglePlcConnectionCommand { get; }

        /// <summary>
        /// 카메라 연결 상태 토글 (개발·디버그용).
        /// </summary>
        public RelayCommand ToggleCameraConnectionCommand { get; }

        /// <summary>
        /// IMQS 연결 상태 토글 (개발·디버그용).
        /// </summary>
        public RelayCommand ToggleImqsConnectionCommand { get; }

        /// <summary>
        /// RFID 연결 상태 토글 (개발·디버그용).
        /// </summary>
        public RelayCommand ToggleRfidConnectionCommand { get; }

        // ───────────────── 생성자 ─────────────────

        public ConnectionStatePanelViewModel()
        {
            TogglePlcConnectionCommand    = new RelayCommand(() => IsPlcConnected    = !IsPlcConnected);
            ToggleCameraConnectionCommand = new RelayCommand(() => IsCameraConnected = !IsCameraConnected);
            ToggleImqsConnectionCommand   = new RelayCommand(() => IsImqsConnected   = !IsImqsConnected);
            ToggleRfidConnectionCommand   = new RelayCommand(() => IsRfidConnected   = !IsRfidConnected);
        }
    }
}
