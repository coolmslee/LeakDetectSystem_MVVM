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
        private string _plcIpAddress = "192.168.0.10";
        private int _plcPort = 502;
        private int _plcStationNumber = 1;

        private string _plcHeartBeatAddress = "0";
        private string _plcBottleRollingAddress = "1";
        private string _plcTopInspectRequestAddress = "2";
        private string _plcSideInspectRequestAddress = "3";
        private string _plcBottleExistsAddress = "10";

        private string _pcHeartBeatAddress = "16";
        private string _pcVisionReadyAddress = "17";
        private string _pcTopInspectDoneAddress = "18";
        private string _pcSideInspectDoneAddress = "19";
        private string _pcQrRequestAddress = "27";

        private string _modelName = "DEFAULT";
        private string _modelPath = string.Empty;
        private string _saveDirectory = string.Empty;
        private string _logPath = string.Empty;
        private bool _isLogEnabled = true;

        private bool _isImqsEnabled;
        private string _imqsIpAddress = "192.168.0.20";
        private int _imqsPort = 5000;

        private bool _isRfidEnabled;
        private string _rfidPort = "COM3";
        private int _rfidBaudRate = 9600;

        private bool _isPrinterEnabled;
        private string _printerIpAddress = "192.168.0.30";
        private int _printerPort = 9100;

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

        public string PlcIpAddress
        {
            get => _plcIpAddress;
            set => SetProperty(ref _plcIpAddress, value);
        }

        public int PlcPort
        {
            get => _plcPort;
            set => SetProperty(ref _plcPort, value);
        }

        public int PlcStationNumber
        {
            get => _plcStationNumber;
            set => SetProperty(ref _plcStationNumber, value);
        }

        public string PlcHeartBeatAddress
        {
            get => _plcHeartBeatAddress;
            set => SetProperty(ref _plcHeartBeatAddress, value);
        }

        public string PlcBottleRollingAddress
        {
            get => _plcBottleRollingAddress;
            set => SetProperty(ref _plcBottleRollingAddress, value);
        }

        public string PlcTopInspectRequestAddress
        {
            get => _plcTopInspectRequestAddress;
            set => SetProperty(ref _plcTopInspectRequestAddress, value);
        }

        public string PlcSideInspectRequestAddress
        {
            get => _plcSideInspectRequestAddress;
            set => SetProperty(ref _plcSideInspectRequestAddress, value);
        }

        public string PlcBottleExistsAddress
        {
            get => _plcBottleExistsAddress;
            set => SetProperty(ref _plcBottleExistsAddress, value);
        }

        public string PcHeartBeatAddress
        {
            get => _pcHeartBeatAddress;
            set => SetProperty(ref _pcHeartBeatAddress, value);
        }

        public string PcVisionReadyAddress
        {
            get => _pcVisionReadyAddress;
            set => SetProperty(ref _pcVisionReadyAddress, value);
        }

        public string PcTopInspectDoneAddress
        {
            get => _pcTopInspectDoneAddress;
            set => SetProperty(ref _pcTopInspectDoneAddress, value);
        }

        public string PcSideInspectDoneAddress
        {
            get => _pcSideInspectDoneAddress;
            set => SetProperty(ref _pcSideInspectDoneAddress, value);
        }

        public string PcQrRequestAddress
        {
            get => _pcQrRequestAddress;
            set => SetProperty(ref _pcQrRequestAddress, value);
        }

        public string ModelName
        {
            get => _modelName;
            set => SetProperty(ref _modelName, value);
        }

        public string ModelPath
        {
            get => _modelPath;
            set => SetProperty(ref _modelPath, value);
        }

        public string SaveDirectory
        {
            get => _saveDirectory;
            set => SetProperty(ref _saveDirectory, value);
        }

        public string LogPath
        {
            get => _logPath;
            set => SetProperty(ref _logPath, value);
        }

        public bool IsLogEnabled
        {
            get => _isLogEnabled;
            set => SetProperty(ref _isLogEnabled, value);
        }

        public bool IsImqsEnabled
        {
            get => _isImqsEnabled;
            set => SetProperty(ref _isImqsEnabled, value);
        }

        public string ImqsIpAddress
        {
            get => _imqsIpAddress;
            set => SetProperty(ref _imqsIpAddress, value);
        }

        public int ImqsPort
        {
            get => _imqsPort;
            set => SetProperty(ref _imqsPort, value);
        }

        public bool IsRfidEnabled
        {
            get => _isRfidEnabled;
            set => SetProperty(ref _isRfidEnabled, value);
        }

        public string RfidPort
        {
            get => _rfidPort;
            set => SetProperty(ref _rfidPort, value);
        }

        public int RfidBaudRate
        {
            get => _rfidBaudRate;
            set => SetProperty(ref _rfidBaudRate, value);
        }

        public bool IsPrinterEnabled
        {
            get => _isPrinterEnabled;
            set => SetProperty(ref _isPrinterEnabled, value);
        }

        public string PrinterIpAddress
        {
            get => _printerIpAddress;
            set => SetProperty(ref _printerIpAddress, value);
        }

        public int PrinterPort
        {
            get => _printerPort;
            set => SetProperty(ref _printerPort, value);
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
            PlcIpAddress = "192.168.0.10";
            PlcPort = 502;
            PlcStationNumber = 1;

            PlcHeartBeatAddress = "0";
            PlcBottleRollingAddress = "1";
            PlcTopInspectRequestAddress = "2";
            PlcSideInspectRequestAddress = "3";
            PlcBottleExistsAddress = "10";

            PcHeartBeatAddress = "16";
            PcVisionReadyAddress = "17";
            PcTopInspectDoneAddress = "18";
            PcSideInspectDoneAddress = "19";
            PcQrRequestAddress = "27";

            ModelName = "DEFAULT";
            ModelPath = string.Empty;
            SaveDirectory = string.Empty;
            LogPath = string.Empty;
            IsLogEnabled = true;

            IsImqsEnabled = false;
            ImqsIpAddress = "192.168.0.20";
            ImqsPort = 5000;

            IsRfidEnabled = false;
            RfidPort = "COM3";
            RfidBaudRate = 9600;

            IsPrinterEnabled = false;
            PrinterIpAddress = "192.168.0.30";
            PrinterPort = 9100;

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
