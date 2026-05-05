using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
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

        /// <summary>CAM1~CAM4 카메라 설정 (Use + IP). IsConfigured = Use && IP 입력됨</summary>
        public ObservableCollection<CameraConfig> Cameras { get; } = new()
        {
            new CameraConfig { Index = 1 },
            new CameraConfig { Index = 2 },
            new CameraConfig { Index = 3 },
            new CameraConfig { Index = 4 },
        };

        // Commands
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand ResetToDefaultCommand { get; }
        public RelayCommand BrowseSaveDirectoryCommand { get; }

        public SettingTabViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetToDefaultCommand = new RelayCommand(ResetToDefault);
            BrowseSaveDirectoryCommand = new RelayCommand(BrowseSaveDirectory);
        }

        private void SaveSettings()
        {
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
            foreach (var cam in Cameras)
            {
                cam.Use = false;
                cam.Ip = string.Empty;
            }
        }

        private void BrowseSaveDirectory()
        {
        }
    }
}
