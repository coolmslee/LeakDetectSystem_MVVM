using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class MainTabViewModel : ViewModelBase, IDisposable
    {
        private readonly ObservableCollection<CameraConfig> _cameras;
        private string _statusText = "모니터링 대기 중";
        private bool _disposed;

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
            _cameras      = cameras;
            StationGroup  = new StationGroupViewModel(cameras);
            Dashboard     = new MainTopDashboardViewModel(cameras, dialogService);
            SignalProcess = new SignalProcessPanelViewModel(statusCallback, dialogService);

            // 카메라 설정 변경 구독 – ConnectionState 동기화
            foreach (var cam in _cameras)
                cam.PropertyChanged += OnCameraPropertyChanged;
            _cameras.CollectionChanged += OnCamerasCollectionChanged;

            // 초기 상태 동기화
            SyncCameraConnectionState();
            SignalProcess.IsPlcPass = Dashboard.IsPlcPass;

            // Dashboard의 물류PASS 변경을 SignalProcess에 전파
            Dashboard.PropertyChanged += OnDashboardPropertyChanged;

            // ConnectionState의 카메라 연결 변경을 SignalProcess에 전파
            ConnectionState.PropertyChanged += OnConnectionStatePropertyChanged;
        }

        // 카메라 설정(IsConfigured) 변경 시 연결 상태 재계산
        private void OnCameraPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CameraConfig.IsConfigured))
                SyncCameraConnectionState();
        }

        private void OnCamerasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CameraConfig cam in e.NewItems)
                    cam.PropertyChanged += OnCameraPropertyChanged;
            if (e.OldItems != null)
                foreach (CameraConfig cam in e.OldItems)
                    cam.PropertyChanged -= OnCameraPropertyChanged;
            SyncCameraConnectionState();
        }

        /// <summary>
        /// 카메라가 1대 이상 설정(Use=true, IP 입력)되어 있으면 카메라 연결 상태를 true로 표시합니다.
        /// </summary>
        private void SyncCameraConnectionState()
        {
            ConnectionState.IsCameraConnected = _cameras.Any(c => c.IsConfigured);
            SignalProcess.IsCameraConnected   = ConnectionState.IsCameraConnected;
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

        public void Dispose()
        {
            if (_disposed) return;
            foreach (var cam in _cameras)
                cam.PropertyChanged -= OnCameraPropertyChanged;
            _cameras.CollectionChanged      -= OnCamerasCollectionChanged;
            Dashboard.PropertyChanged       -= OnDashboardPropertyChanged;
            ConnectionState.PropertyChanged -= OnConnectionStatePropertyChanged;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
