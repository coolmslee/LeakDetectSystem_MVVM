using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 메인 탭(모니터링 탭) 화면에 대한 ViewModel.
    /// 여러 스테이션의 목록을 관리하고, 모니터링 시작/중지 명령을 제공합니다.
    /// </summary>
    public class MainTabViewModel : ViewModelBase
    {
        private bool _isMonitoring;
        private string _statusText = "모니터링 대기 중";
        private StationViewModel? _selectedStation;

        public ObservableCollection<StationViewModel> Stations { get; } = new();

        public StationViewModel? SelectedStation
        {
            get => _selectedStation;
            set => SetProperty(ref _selectedStation, value);
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value, () =>
            {
                StatusText = _isMonitoring ? "모니터링 중..." : "모니터링 대기 중";
                OnPropertyChanged(nameof(MonitoringButtonText));
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
        public RelayCommand<StationViewModel> SelectStationCommand { get; }

        public MainTabViewModel()
        {
            ToggleMonitoringCommand = new RelayCommand(ToggleMonitoring);
            RefreshCommand = new RelayCommand(RefreshStations);
            SelectStationCommand = new RelayCommand<StationViewModel>(station => SelectedStation = station);

            LoadSampleStations();
        }

        private void ToggleMonitoring()
        {
            IsMonitoring = !IsMonitoring;
            foreach (var station in Stations)
                station.IsMonitoring = IsMonitoring;
        }

        private void RefreshStations()
        {
            // 실제 구현에서는 서비스/레포지토리를 통해 데이터를 새로 고침합니다.
            StatusText = "데이터 새로 고침 완료";
        }

        private void LoadSampleStations()
        {
            var sampleData = new[]
            {
                new LeakInfoModel { StationId = 1, StationName = "Station 1 - Line A", PressureValue = 102.5, Threshold = 110.0, LastMeasuredAt = DateTime.Now },
                new LeakInfoModel { StationId = 2, StationName = "Station 2 - Line B", IsLeakDetected = true, PressureValue = 125.3, Threshold = 110.0, LastMeasuredAt = DateTime.Now },
                new LeakInfoModel { StationId = 3, StationName = "Station 3 - Line C", PressureValue = 98.7, Threshold = 110.0, LastMeasuredAt = DateTime.Now },
                new LeakInfoModel { StationId = 4, StationName = "Station 4 - Line D", PressureValue = 107.1, Threshold = 110.0, LastMeasuredAt = DateTime.Now },
            };

            foreach (var model in sampleData)
                Stations.Add(StationViewModel.FromModel(model));
        }
    }
}
