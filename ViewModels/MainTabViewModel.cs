using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 메인 탭(모니터링 탭) 화면에 대한 ViewModel.
    /// StationGroupViewModel 을 통해 ST1~ST4 스테이션 영역을 관리하고,
    /// ConnectionStatePanelViewModel 을 통해 연결상태/신호 패널을 관리하며,
    /// 모니터링 시작/중지 명령을 제공합니다.
    /// </summary>
    public class MainTabViewModel : ViewModelBase
    {
        private bool _isMonitoring;
        private string _statusText = "모니터링 대기 중";

        /// <summary>
        /// ST1~ST4 스테이션 그룹 ViewModel.
        /// StationGroupView.xaml 에 DataContext 로 바인딩됩니다.
        /// </summary>
        public StationGroupViewModel StationGroup { get; } = new();

        /// <summary>
        /// 연결상태/요청신호 패널 ViewModel.
        /// ConnectionStatePanelView.xaml 에 DataContext 로 바인딩됩니다.
        /// </summary>
        public ConnectionStatePanelViewModel ConnectionState { get; } = new();

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value, () =>
            {
                StatusText = _isMonitoring ? "모니터링 중..." : "모니터링 대기 중";
                OnPropertyChanged(nameof(MonitoringButtonText));
                // 모니터링 시작/중지 시 통신 상태 플래그 반영
                ConnectionState.IsCommunicationActive = _isMonitoring;
            });
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public string MonitoringButtonText => IsMonitoring ? "모니터링 중지" : "모니터링 시작";

        // Commands
        public RelayCommand ToggleMonitoringCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public MainTabViewModel()
        {
            ToggleMonitoringCommand = new RelayCommand(ToggleMonitoring);
            RefreshCommand = new RelayCommand(RefreshStations);
        }

        private void ToggleMonitoring()
        {
            IsMonitoring = !IsMonitoring;
            foreach (var station in StationGroup.Stations)
                station.IsMonitoring = IsMonitoring;
        }

        private void RefreshStations()
        {
            // 실제 구현에서는 서비스/레포지토리를 통해 데이터를 새로 고침합니다.
            StatusText = "데이터 새로 고침 완료";
        }
    }
}
