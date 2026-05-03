using System.Windows.Threading;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 상단 TitleBar(메뉴/시간/Exit) 영역의 ViewModel.
    /// CurrentTime 속성을 1초마다 갱신하고, 각 메뉴 버튼에 대한 Command를 제공합니다.
    /// FooterStatus 업데이트는 생성 시 주입된 콜백(Action&lt;string&gt;)을 통해 MainWindowViewModel에 전달합니다.
    /// </summary>
    public class TitleBarViewModel : ViewModelBase, IDisposable
    {
        private readonly Action<string>? _statusCallback;
        private readonly DispatcherTimer _timer;
        private string _currentTime = string.Empty;

        // ───────────────── 속성 ─────────────────

        /// <summary>현재 날짜/시간 문자열 (매 초 갱신)</summary>
        public string CurrentTime
        {
            get => _currentTime;
            private set => SetProperty(ref _currentTime, value);
        }

        // ───────────────── Commands ─────────────────

        /// <summary>프로그램 종료</summary>
        public RelayCommand ExitCommand { get; }

        /// <summary>LOG 화면 열기</summary>
        public RelayCommand ShowLogCommand { get; }

        /// <summary>RFID 화면 열기</summary>
        public RelayCommand ShowRfidCommand { get; }

        /// <summary>MENU 열기</summary>
        public RelayCommand OpenMenuCommand { get; }

        /// <summary>PLC 설정 열기</summary>
        public RelayCommand OpenPlcCommand { get; }

        /// <summary>Model 설정 열기</summary>
        public RelayCommand OpenModelCommand { get; }

        /// <summary>Camera 설정 열기</summary>
        public RelayCommand OpenCameraCommand { get; }

        /// <summary>Grab 설정 열기</summary>
        public RelayCommand OpenGrabCommand { get; }

        /// <summary>Light 설정 열기</summary>
        public RelayCommand OpenLightCommand { get; }

        // ───────────────── 생성자 ─────────────────

        /// <summary>
        /// 기본 생성자 (콜백 없음 – 디자인 타임 / 단독 사용).
        /// </summary>
        public TitleBarViewModel() : this(null) { }

        /// <summary>
        /// FooterStatus 콜백을 주입받는 생성자.
        /// </summary>
        /// <param name="statusCallback">FooterStatus를 업데이트할 콜백. null이면 무시.</param>
        public TitleBarViewModel(Action<string>? statusCallback)
        {
            _statusCallback = statusCallback;

            // 현재 시간 초기화
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 1초 간격 타이머
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _timer.Start();

            // Command 초기화
            ExitCommand    = new RelayCommand(() =>
            {
                ReportStatus("프로그램 종료");
                System.Windows.Application.Current.Shutdown();
            });

            ShowLogCommand   = new RelayCommand(() => ReportStatus("LOG 창 열기"));
            ShowRfidCommand  = new RelayCommand(() => ReportStatus("RFID 창 열기"));
            OpenMenuCommand  = new RelayCommand(() => ReportStatus("MENU 열기"));
            OpenPlcCommand   = new RelayCommand(() => ReportStatus("PLC 설정 열기"));
            OpenModelCommand = new RelayCommand(() => ReportStatus("Model 설정 열기"));
            OpenCameraCommand = new RelayCommand(() => ReportStatus("Camera 설정 열기"));
            OpenGrabCommand  = new RelayCommand(() => ReportStatus("Grab 설정 열기"));
            OpenLightCommand = new RelayCommand(() => ReportStatus("Light 설정 열기"));
        }

        // ───────────────── 내부 헬퍼 ─────────────────

        private void ReportStatus(string message) => _statusCallback?.Invoke(message);

        // ───────────────── IDisposable ─────────────────

        private bool _disposed;

        /// <summary>타이머를 중지하고 리소스를 해제합니다.</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _timer.Stop();
            _disposed = true;
        }
    }
}
