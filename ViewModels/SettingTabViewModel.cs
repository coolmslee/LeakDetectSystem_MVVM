using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 설정 탭 화면에 대한 ViewModel.
    /// 장치 연결 설정, 임계값 설정 등 시스템 설정 항목을 관리합니다.
    /// </summary>
    public class SettingTabViewModel : ViewModelBase
    {
        private string _devicePort = "COM1";
        private int _baudRate = 9600;
        private double _defaultThreshold = 110.0;
        private bool _isAutoSaveEnabled;
        private string _saveDirectory = string.Empty;
        private bool _isSaved;

        public string DevicePort
        {
            get => _devicePort;
            set => SetProperty(ref _devicePort, value);
        }

        public int BaudRate
        {
            get => _baudRate;
            set => SetProperty(ref _baudRate, value);
        }

        public double DefaultThreshold
        {
            get => _defaultThreshold;
            set => SetProperty(ref _defaultThreshold, value);
        }

        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set => SetProperty(ref _isAutoSaveEnabled, value);
        }

        public string SaveDirectory
        {
            get => _saveDirectory;
            set => SetProperty(ref _saveDirectory, value);
        }

        public bool IsSaved
        {
            get => _isSaved;
            set => SetProperty(ref _isSaved, value);
        }

        public IReadOnlyList<int> BaudRateOptions { get; } = new[] { 4800, 9600, 19200, 38400, 57600, 115200 };

        // Commands
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand ResetToDefaultCommand { get; }
        public RelayCommand BrowseSaveDirectoryCommand { get; }

        public SettingTabViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings, () => !IsSaved);
            ResetToDefaultCommand = new RelayCommand(ResetToDefault);
            BrowseSaveDirectoryCommand = new RelayCommand(BrowseSaveDirectory);
        }

        private void SaveSettings()
        {
            // 실제 구현에서는 설정 파일이나 레지스트리에 저장합니다.
            IsSaved = true;
        }

        private void ResetToDefault()
        {
            DevicePort = "COM1";
            BaudRate = 9600;
            DefaultThreshold = 110.0;
            IsAutoSaveEnabled = false;
            SaveDirectory = string.Empty;
            IsSaved = false;
        }

        private void BrowseSaveDirectory()
        {
            // 실제 구현에서는 IDialogService를 주입받아 폴더 선택 다이얼로그를 열어야 합니다.
            // 예: SaveDirectory = _dialogService.ShowFolderBrowserDialog();
        }
    }
}
