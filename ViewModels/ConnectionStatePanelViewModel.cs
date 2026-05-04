using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 연결상태 패널 ViewModel.
    /// 원본(LeakDetectionSystem) 기준:
    ///   MainWindow.xaml SearchPanel 내 "연결상태" 섹션의 LIGHT / PLC 두 항목에 대응합니다.
    ///   statusLight(name=statusLight), statusPLC(name=statusPLC) Ellipse 색상을 MVVM으로 분리합니다.
    /// </summary>
    public class ConnectionStatePanelViewModel : ViewModelBase
    {
        private bool _isLightConnected;
        private bool _isPlcConnected;

        // ───────────────── 속성 ─────────────────

        /// <summary>LIGHT 연결 상태. true = 연결됨(Green), false = 미연결(Gray)</summary>
        public bool IsLightConnected
        {
            get => _isLightConnected;
            set => SetProperty(ref _isLightConnected, value);
        }

        /// <summary>PLC 연결 상태. true = 연결됨(Green), false = 미연결(Gray)</summary>
        public bool IsPlcConnected
        {
            get => _isPlcConnected;
            set => SetProperty(ref _isPlcConnected, value);
        }

        // ───────────────── Commands ─────────────────

        /// <summary>LIGHT 연결 상태 토글 (테스트용)</summary>
        public RelayCommand ToggleLightCommand { get; }

        /// <summary>PLC 연결 상태 토글 (테스트용)</summary>
        public RelayCommand TogglePlcCommand { get; }

        // ───────────────── 생성자 ─────────────────

        public ConnectionStatePanelViewModel()
        {
            ToggleLightCommand = new RelayCommand(() => IsLightConnected = !IsLightConnected);
            TogglePlcCommand   = new RelayCommand(() => IsPlcConnected   = !IsPlcConnected);
        }
    }
}
