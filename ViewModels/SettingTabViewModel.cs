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

        private int _bottleTurnTime = 500;
        private int _inspectReqTime = 300;
        private int _inspectEndTime = 200;

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

        public int BottleTurnTime
        {
            get => _bottleTurnTime;
            set => SetProperty(ref _bottleTurnTime, value);
        }

        public int InspectReqTime
        {
            get => _inspectReqTime;
            set => SetProperty(ref _inspectReqTime, value);
        }

        public int InspectEndTime
        {
            get => _inspectEndTime;
            set => SetProperty(ref _inspectEndTime, value);
        }

        public bool IsSaved
        {
            get => _isSaved;
            set => SetProperty(ref _isSaved, value);
        }

        public IReadOnlyList<int> BaudRateOptions { get; } = new[] { 4800, 9600, 19200, 38400, 57600, 115200 };

        /// <summary>CAM1~CAM4 카메라 설정. Setting 탭 UI에서는 제거되었으며 CameraDialog를 통해 관리됩니다.</summary>
        public ObservableCollection<CameraConfig> Cameras { get; } = new()
        {
            new CameraConfig { Index = 1 },
            new CameraConfig { Index = 2 },
            new CameraConfig { Index = 3 },
            new CameraConfig { Index = 4 },
        };

        /// <summary>DIO Input 상태 (0~15), read-only 표시용</summary>
        public ObservableCollection<bool> DioInputStates { get; } =
            new(Enumerable.Repeat(false, 16));

        /// <summary>DIO Output 상태 (0~15), ToggleButton 바인딩용</summary>
        public ObservableCollection<bool> DioOutputStates { get; } =
            new(Enumerable.Repeat(false, 16));

        public RelayCommand SaveMemoryCommand { get; }
        public RelayCommand SaveTimesCommand { get; }
        public RelayCommand<int> ToggleDioOutputCommand { get; }

        public SettingTabViewModel()
        {
            SaveMemoryCommand = new RelayCommand(SaveMemory);
            SaveTimesCommand = new RelayCommand(SaveTimes);
            ToggleDioOutputCommand = new RelayCommand<int>(ToggleDioOutput);
        }

        private void SaveMemory()
        {
            IsSaved = true;
        }

        private void SaveTimes()
        {
            IsSaved = true;
        }

        private void ToggleDioOutput(int index)
        {
            if (index >= 0 && index < DioOutputStates.Count)
                DioOutputStates[index] = !DioOutputStates[index];
        }
    }
}
