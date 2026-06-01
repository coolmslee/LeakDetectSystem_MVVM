using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class LightDialogViewModel : DialogViewModelBase
    {
        private readonly IDialogService _dialogService;
        private string _statusMessage = "사용 여부와 밝기를 조정한 뒤 적용을 눌러 반영하세요.";

        public LightDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Channels = new ObservableCollection<LightChannelSettingViewModel>
            {
                new("CH1", true, 100),
                new("CH2", true, 100),
                new("CH3", false, 0),
                new("CH4", false, 0)
            };

            ApplyCommand = new RelayCommand(Apply);
        }

        public ObservableCollection<LightChannelSettingViewModel> Channels { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public RelayCommand ApplyCommand { get; }

        private void Apply()
        {
            foreach (LightChannelSettingViewModel channel in Channels)
            {
                channel.Apply();
            }

            StatusMessage = $"마지막 적용: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _dialogService.ShowMessage("누액검사 조명 설정이 적용되었습니다.", "Light 설정");
        }
    }

    public class LightChannelSettingViewModel : ViewModelBase
    {
        private readonly string _channelName;
        private bool _isEnabled;
        private bool _currentEnabled;
        private int _brightness;
        private int _currentBrightness;

        public LightChannelSettingViewModel(string channelName, bool isEnabled, int brightness)
        {
            _channelName = channelName;
            _isEnabled = isEnabled;
            _brightness = NormalizeBrightness(brightness);
            _currentEnabled = isEnabled;
            _currentBrightness = isEnabled ? _brightness : 0;
        }

        public string ChannelName => _channelName;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, () => OnPropertyChanged(nameof(PendingValueText)));
        }

        public int Brightness
        {
            get => _brightness;
            set => SetProperty(ref _brightness, NormalizeBrightness(value), () => OnPropertyChanged(nameof(PendingValueText)));
        }

        public bool CurrentEnabled => _currentEnabled;

        public int CurrentBrightness => _currentBrightness;

        public string PendingValueText => IsEnabled ? $"{Brightness:000}" : "OFF";

        public string CurrentValueText => CurrentEnabled ? $"현재 {CurrentBrightness:000}" : "현재 OFF";

        public void Apply()
        {
            int appliedBrightness = IsEnabled ? Brightness : 0;

            bool stateChanged = SetProperty(ref _currentEnabled, IsEnabled, nameof(CurrentEnabled));
            bool valueChanged = SetProperty(ref _currentBrightness, appliedBrightness, nameof(CurrentBrightness));

            if (stateChanged || valueChanged)
            {
                OnPropertyChanged(nameof(CurrentValueText));
            }
        }

        private static int NormalizeBrightness(int value) => Math.Clamp(value, 0, 255);
    }
}
