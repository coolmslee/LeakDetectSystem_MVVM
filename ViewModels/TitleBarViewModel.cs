using System.Windows.Threading;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class TitleBarViewModel : ViewModelBase, IDisposable
    {
        private readonly Action<string>? _statusCallback;
        private readonly IDialogService? _dialogService;
        private readonly DispatcherTimer _timer;
        private string _currentTime = string.Empty;

        public string CurrentTime
        {
            get => _currentTime;
            private set => SetProperty(ref _currentTime, value);
        }

        public RelayCommand ExitCommand { get; }
        public RelayCommand ShowLogCommand { get; }
        public RelayCommand ShowRfidCommand { get; }
        public RelayCommand OpenMenuCommand { get; }
        public RelayCommand OpenPlcCommand { get; }
        public RelayCommand OpenModelCommand { get; }
        public RelayCommand OpenCameraCommand { get; }
        public RelayCommand OpenGrabCommand { get; }
        public RelayCommand OpenLightCommand { get; }

        public TitleBarViewModel() : this(null, null) { }

        public TitleBarViewModel(Action<string>? statusCallback) : this(statusCallback, null) { }

        public TitleBarViewModel(Action<string>? statusCallback, IDialogService? dialogService)
        {
            _statusCallback = statusCallback;
            _dialogService = dialogService;

            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _timer.Start();

            ExitCommand = new RelayCommand(() =>
            {
                ReportStatus("프로그램 종료");
                System.Windows.Application.Current.Shutdown();
            });

            ShowLogCommand    = new RelayCommand(() => { _dialogService?.ShowLogDialog();    ReportStatus("LOG 창 열기"); });
            ShowRfidCommand   = new RelayCommand(() => ReportStatus("RFID 창 열기"));
            OpenMenuCommand   = new RelayCommand(() => ReportStatus("MENU 열기"));
            OpenPlcCommand    = new RelayCommand(() => { _dialogService?.ShowPlcDialog();    ReportStatus("PLC 설정 열기"); });
            OpenModelCommand  = new RelayCommand(() => { _dialogService?.ShowModelDialog();  ReportStatus("Model 설정 열기"); });
            OpenCameraCommand = new RelayCommand(() => { _dialogService?.ShowCameraDialog(); ReportStatus("Camera 설정 열기"); });
            OpenGrabCommand   = new RelayCommand(() => { _dialogService?.ShowGrabDialog();   ReportStatus("Grab 설정 열기"); });
            OpenLightCommand  = new RelayCommand(() => { _dialogService?.ShowLightDialog();  ReportStatus("Light 설정 열기"); });
        }

        private void ReportStatus(string message) => _statusCallback?.Invoke(message);

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _timer.Stop();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
