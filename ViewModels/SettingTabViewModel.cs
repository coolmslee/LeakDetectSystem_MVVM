using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class SettingTabViewModel : ViewModelBase
    {
        private string _plcIpAddress = "192.168.0.10";
        private string _plcPort = "502";

        private string _plcStartAddress = "M2500";
        private string _pcStartAddress = "M2550";

        private string _plcHeartBeatAddress = "0";
        private string _plcBottleRollingAddress = "1";
        private string _plcInspectRequestAddress = "2";
        private string _plcReset1ReqAddress = "3";
        private string _plcReset2AckAddress = "4";
        private string _plcBottleExistsAddress = "5";

        private string _pcHeartBeatAddress = "16";
        private string _pcVisionReadyAddress = "17";
        private string _pcInspectDoneAddress = "18";
        private string _pcReset1AckAddress = "19";
        private string _pcReset2ReqAddress = "20";
        private string _pcBottleDataReqAddress = "27";

        private int _bottleTurnTime = 500;
        private int _inspectReqTime = 300;
        private int _inspectEndTime = 200;

        private bool _isSaved;

        /// <summary>PLC IP – PLC 통신 메뉴에서 설정된 값을 표시(읽기 전용)</summary>
        public string PlcIpAddress
        {
            get => _plcIpAddress;
            private set => SetProperty(ref _plcIpAddress, value);
        }

        /// <summary>PLC Port – PLC 통신 메뉴에서 설정된 값을 표시(읽기 전용)</summary>
        public string PlcPort
        {
            get => _plcPort;
            private set => SetProperty(ref _plcPort, value);
        }

        public string PlcStartAddress
        {
            get => _plcStartAddress;
            set => SetProperty(ref _plcStartAddress, value);
        }

        public string PcStartAddress
        {
            get => _pcStartAddress;
            set => SetProperty(ref _pcStartAddress, value);
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

        public string PlcInspectRequestAddress
        {
            get => _plcInspectRequestAddress;
            set => SetProperty(ref _plcInspectRequestAddress, value);
        }

        public string PlcReset1ReqAddress
        {
            get => _plcReset1ReqAddress;
            set => SetProperty(ref _plcReset1ReqAddress, value);
        }

        public string PlcReset2AckAddress
        {
            get => _plcReset2AckAddress;
            set => SetProperty(ref _plcReset2AckAddress, value);
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

        public string PcInspectDoneAddress
        {
            get => _pcInspectDoneAddress;
            set => SetProperty(ref _pcInspectDoneAddress, value);
        }

        public string PcReset1AckAddress
        {
            get => _pcReset1AckAddress;
            set => SetProperty(ref _pcReset1AckAddress, value);
        }

        public string PcReset2ReqAddress
        {
            get => _pcReset2ReqAddress;
            set => SetProperty(ref _pcReset2ReqAddress, value);
        }

        public string PcBottleDataReqAddress
        {
            get => _pcBottleDataReqAddress;
            set => SetProperty(ref _pcBottleDataReqAddress, value);
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

        /// <summary>CAM1~CAM4 카메라 설정. Setting 탭 UI에서는 제거되었으며 CameraDialog를 통해 관리됩니다.</summary>
        public ObservableCollection<CameraConfig> Cameras { get; }

        /// <summary>PLC Memory 상태 (0~15), read-only 표시용</summary>
        public ObservableCollection<bool> DioInputStates { get; } =
            new(Enumerable.Repeat(false, 16));

        /// <summary>PC Memory 상태 (0~15), ToggleButton 바인딩용</summary>
        public ObservableCollection<bool> DioOutputStates { get; } =
            new(Enumerable.Repeat(false, 16));

        /// <summary>조명 채널 목록 – 현재 On/Off 상태 및 밝기 표시용</summary>
        public ObservableCollection<SettingLightChannelViewModel> LightChannels { get; } = new();

        public RelayCommand SaveMemoryCommand { get; }
        public RelayCommand SaveTimesCommand { get; }
        public RelayCommand<int> ToggleDioOutputCommand { get; }
        public RelayCommand<SettingLightChannelViewModel> ToggleLightCommand { get; }

        public SettingTabViewModel()
            : this(new CameraConfigIniService(), new PlcConfigIniService(), new LightConfigIniService()) { }

        public SettingTabViewModel(
            ICameraConfigService cameraConfigService,
            IPlcConfigService plcConfigService,
            ILightConfigService lightConfigService)
        {
            var loaded = cameraConfigService.Load();
            Cameras = loaded.Count > 0
                ? new ObservableCollection<CameraConfig>(loaded)
                : new ObservableCollection<CameraConfig>(
                    Enumerable.Range(1, 4).Select(i => new CameraConfig { Index = i }));

            var plcConfig = plcConfigService.Load();
            _plcIpAddress = plcConfig.IpAddress;
            _plcPort = plcConfig.Port;

            LoadLightChannels(lightConfigService.Load());

            SaveMemoryCommand = new RelayCommand(SaveMemory);
            SaveTimesCommand = new RelayCommand(SaveTimes);
            ToggleDioOutputCommand = new RelayCommand<int>(ToggleDioOutput);
            ToggleLightCommand = new RelayCommand<SettingLightChannelViewModel>(ToggleLight);
        }

        private void LoadLightChannels(LightConfig config)
        {
            LightChannels.Clear();
            foreach (var controller in config.Controllers)
                foreach (var channel in controller.Channels)
                    LightChannels.Add(new SettingLightChannelViewModel(controller.Name, channel));
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

        private void ToggleLight(SettingLightChannelViewModel? channel)
        {
            if (channel != null)
                channel.IsOn = !channel.IsOn;
        }
    }

    /// <summary>Setting 탭에서 조명 채널의 현재 상태를 표시하는 ViewModel</summary>
    public class SettingLightChannelViewModel : ViewModelBase
    {
        private bool _isOn;

        public SettingLightChannelViewModel(string controllerName, LightChannelConfig channel)
        {
            ControllerName = controllerName;
            ChannelName = channel.Name;
            Brightness = channel.Brightness;
            _isOn = channel.Use;
        }

        public string ControllerName { get; }
        public string ChannelName { get; }
        public int Brightness { get; }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (SetProperty(ref _isOn, value))
                    OnPropertyChanged(nameof(StatusText));
            }
        }

        public string DisplayName => $"{ControllerName} - {ChannelName}";
        public string BrightnessText => $"{Brightness:000}";
        public string StatusText => IsOn ? "ON" : "OFF";
    }
}
