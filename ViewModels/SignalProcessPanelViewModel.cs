using LeakDetectSystem_MVVM.Commands;
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

        // ─── MODEL 표시 ───
        private string _modelName = "-";
        public string ModelName { get => _modelName; set => SetProperty(ref _modelName, value); }

        // ─── QRcode 표시 ───
        private string _qrCode = "-";
        public string QrCode { get => _qrCode; set => SetProperty(ref _qrCode, value); }

        // ─── 검사진행상태 ───
        private string _progressText = "대기";
        public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }

        // ─── Commands ───
        public RelayCommand ManualInspectCommand { get; }
        public RelayCommand StopCommand { get; }

        // 디자인 타임 / 테스트용 기본 생성자
        public SignalProcessPanelViewModel() : this(null) { }

        public SignalProcessPanelViewModel(Action<string>? statusCallback)
        {
            _statusCallback = statusCallback;

            ManualInspectCommand = new RelayCommand(OnManualInspect);
            StopCommand          = new RelayCommand(OnStop);
        }

        private void OnManualInspect() => _statusCallback?.Invoke("수동검사");

        private void OnStop() => _statusCallback?.Invoke("수동검사 STOP");
    }
}
