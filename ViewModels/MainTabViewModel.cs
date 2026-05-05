using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class MainTabViewModel : ViewModelBase
    {
        private string _statusText = "모니터링 대기 중";

        public StationGroupViewModel StationGroup { get; }
        public MainTopDashboardViewModel Dashboard { get; }
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
        {
            StationGroup = new StationGroupViewModel(cameras);
            Dashboard    = new MainTopDashboardViewModel(cameras);
        }
    }
}
