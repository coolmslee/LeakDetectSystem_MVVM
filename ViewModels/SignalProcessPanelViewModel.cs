using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// Main 화면 상단 2번째 줄 – Signal/Process 패널 ViewModel.
    /// MODEL / QRcode / 검사진행상태 표시와 수동검사·STOP 커맨드를 담당합니다.
    /// </summary>
    public class SignalProcessPanelViewModel : ViewModelBase
    {
        private readonly Action<string>? _statusCallback;
        private readonly IDialogService? _dialogService;

        // ─── MODEL 표시 ───
        private string _modelName = "-";
        public string ModelName { get => _modelName; set => SetProperty(ref _modelName, value); }

        // ─── QRcode 표시 ───
        private string _qrCode = "-";
        public string QrCode { get => _qrCode; set => SetProperty(ref _qrCode, value); }

        // ─── 검사진행상태 ───
        private string _progressText = "대기";
        public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }

        // ─── 수동검사 활성 상태 (수동검사 ↔ STOP 토글) ───
        private bool _isManualInspecting;
        public bool IsManualInspecting { get => _isManualInspecting; set => SetProperty(ref _isManualInspecting, value); }

        // ─── 수동검사 가능 조건: 물류(PLC) 준비 ───
        private bool _isPlcPass;
        public bool IsPlcPass { get => _isPlcPass; set => SetProperty(ref _isPlcPass, value); }

        // ─── 수동검사 가능 조건: 카메라 연결 ───
        private bool _isCameraConnected;
        public bool IsCameraConnected { get => _isCameraConnected; set => SetProperty(ref _isCameraConnected, value); }

        // ─── Commands ───
        public RelayCommand ManualInspectCommand { get; }
        public RelayCommand StopCommand { get; }

        // 디자인 타임 / 테스트용 기본 생성자
        public SignalProcessPanelViewModel() : this(null) { }

        public SignalProcessPanelViewModel(Action<string>? statusCallback)
            : this(statusCallback, null) { }

        public SignalProcessPanelViewModel(Action<string>? statusCallback, IDialogService? dialogService)
        {
            _statusCallback = statusCallback;
            _dialogService  = dialogService;

            ManualInspectCommand = new RelayCommand(OnManualInspect);
            StopCommand          = new RelayCommand(OnStop);
        }

        private void OnManualInspect()
        {
            var reasons = new List<string>();
            if (!IsPlcPass)        reasons.Add("물류(PLC)가 준비되어 있지 않습니다.");
            if (!IsCameraConnected) reasons.Add("카메라가 연결되어 있지 않습니다.");

            if (reasons.Count > 0)
            {
                var message = "수동검사를 시작할 수 없습니다:\n\n" + string.Join("\n", reasons);
                if (_dialogService != null)
                    _dialogService.ShowError(message, "수동검사 오류");
                else
                    System.Windows.MessageBox.Show(message, "수동검사 오류",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            IsManualInspecting = true;
            ProgressText       = "수동검사 중";
            _statusCallback?.Invoke("수동검사");
        }

        private void OnStop()
        {
            IsManualInspecting = false;
            ProgressText       = "대기";
            _statusCallback?.Invoke("수동검사 STOP");
        }
    }
}
