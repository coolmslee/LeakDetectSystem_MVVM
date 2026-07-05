using System.Collections.ObjectModel;
using System.ComponentModel;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class MainTabViewModel : ViewModelBase
    {
        private string _statusText = "모니터링 대기 중";

        public StationGroupViewModel StationGroup { get; }
        public MainTopDashboardViewModel Dashboard { get; }
        public SignalProcessPanelViewModel SignalProcess { get; }
        public ConnectionStatePanelViewModel ConnectionState { get; } = new();

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        // 기본 생성자 (디자인 타임용)
        public MainTabViewModel() : this(new ObservableCollection<CameraConfig>
        {
            new CameraConfig { Index = 1, Use = true, Ip = "192.168.0.1" },
            new CameraConfig { Index = 2, Use = true, Ip = "192.168.0.2" },
            new CameraConfig { Index = 3 },
            new CameraConfig { Index = 4 },
        }) { }

        public MainTabViewModel(ObservableCollection<CameraConfig> cameras)
            : this(cameras, null) { }

        public MainTabViewModel(ObservableCollection<CameraConfig> cameras, Action<string>? statusCallback)
            : this(cameras, statusCallback, null) { }

        public MainTabViewModel(ObservableCollection<CameraConfig> cameras, Action<string>? statusCallback,
            IDialogService? dialogService)
        {
            StationGroup  = new StationGroupViewModel(cameras);
            Dashboard     = new MainTopDashboardViewModel(cameras, dialogService);
            SignalProcess = new SignalProcessPanelViewModel(statusCallback, dialogService);

            // 초기 상태 동기화
            SignalProcess.IsPlcPass        = Dashboard.IsPlcPass;
            SignalProcess.IsCameraConnected = ConnectionState.IsCameraConnected;

            // Dashboard의 물류PASS 변경을 SignalProcess에 전파
            Dashboard.PropertyChanged += OnDashboardPropertyChanged;

            // ConnectionState의 카메라 연결 변경을 SignalProcess에 전파
            ConnectionState.PropertyChanged += OnConnectionStatePropertyChanged;
        }

        private void OnDashboardPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainTopDashboardViewModel.IsPlcPass))
                SignalProcess.IsPlcPass = Dashboard.IsPlcPass;
        }

        private void OnConnectionStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectionStatePanelViewModel.IsCameraConnected))
                SignalProcess.IsCameraConnected = ConnectionState.IsCameraConnected;
        }
    }
}
