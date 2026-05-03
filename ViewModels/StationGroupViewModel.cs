using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 전체 스테이션 영역(ST1~ST4)을 관리하는 ViewModel.
    /// StationGroupView.xaml 에 DataContext 로 바인딩됩니다.
    ///
    /// 원본(LeakDetectSystem) 기준:
    ///   MainWindow.xaml 의 ST1~ST4 Camera/Station 패널 영역 전체에 대응합니다.
    /// </summary>
    public class StationGroupViewModel : ViewModelBase
    {
        private StationCardViewModel? _selectedStation;

        /// <summary>ST1~ST4 스테이션 카드 목록</summary>
        public ObservableCollection<StationCardViewModel> Stations { get; } = new();

        /// <summary>현재 선택된 스테이션 (상세 정보 표시 등에 활용)</summary>
        public StationCardViewModel? SelectedStation
        {
            get => _selectedStation;
            set => SetProperty(ref _selectedStation, value);
        }

        /// <summary>스테이션 선택 커맨드</summary>
        public RelayCommand<StationCardViewModel> SelectStationCommand { get; }

        public StationGroupViewModel()
        {
            SelectStationCommand = new RelayCommand<StationCardViewModel>(s => SelectedStation = s);
            LoadSampleStations();
        }

        /// <summary>샘플 ST1~ST4 데이터를 초기화합니다 (실제 운용 시 서비스 주입으로 대체).</summary>
        private void LoadSampleStations()
        {
            Stations.Add(new StationCardViewModel
            {
                StationId = 1,
                StationName = "ST1",
                PressureValue = 102.5,
                Threshold = 110.0,
                ResultState = StationResultState.OK,
            });
            Stations.Add(new StationCardViewModel
            {
                StationId = 2,
                StationName = "ST2",
                PressureValue = 125.3,
                Threshold = 110.0,
                ResultState = StationResultState.NG,
            });
            Stations.Add(new StationCardViewModel
            {
                StationId = 3,
                StationName = "ST3",
                PressureValue = 98.7,
                Threshold = 110.0,
                ResultState = StationResultState.OK,
            });
            Stations.Add(new StationCardViewModel
            {
                StationId = 4,
                StationName = "ST4",
                PressureValue = 107.1,
                Threshold = 110.0,
                ResultState = StationResultState.OK,
            });
        }
    }
}
