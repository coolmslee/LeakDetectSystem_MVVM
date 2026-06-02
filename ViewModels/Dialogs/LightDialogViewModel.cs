using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class LightDialogViewModel : DialogViewModelBase
    {
        private readonly ILightConfigService _lightConfigService;
        private readonly IDialogService _dialogService;
        private string _statusMessage = "컨트롤러를 선택하고 포트/채널 설정을 편집하세요.";
        private LightControllerSettingViewModel? _selectedController;
        private int _nextControllerNumber = 1;

        public LightDialogViewModel()
            : this(new LightConfigIniService(), new DialogService())
        {
        }

        public LightDialogViewModel(ILightConfigService lightConfigService, IDialogService dialogService)
        {
            _lightConfigService = lightConfigService;
            _dialogService = dialogService;

            Controllers = new ObservableCollection<LightControllerSettingViewModel>();

            AddControllerCommand = new RelayCommand(AddController);
            RemoveControllerCommand = new RelayCommand(RemoveSelectedController, () => SelectedController != null && Controllers.Count > 1);
            ApplyCommand = new RelayCommand(Apply);
            SaveCommand = new RelayCommand(Save);

            LoadFromIni();
        }

        public ObservableCollection<LightControllerSettingViewModel> Controllers { get; }

        public LightControllerSettingViewModel? SelectedController
        {
            get => _selectedController;
            set
            {
                if (SetProperty(ref _selectedController, value))
                {
                    OnPropertyChanged(nameof(HasSelectedController));
                    RemoveControllerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasSelectedController => SelectedController != null;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public RelayCommand AddControllerCommand { get; }
        public RelayCommand RemoveControllerCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand SaveCommand { get; }

        private void LoadFromIni()
        {
            try
            {
                ApplyConfig(_lightConfigService.Load());
                StatusMessage = $"{Controllers.Count}개의 조명 컨트롤러 설정을 불러왔습니다.";
            }
            catch (Exception ex)
            {
                ApplyConfig(LightConfig.CreateDefault());
                StatusMessage = $"설정 로드 실패: {ex.Message}";
            }
        }

        private void ApplyConfig(LightConfig config)
        {
            Controllers.Clear();

            foreach (LightControllerConfig controller in config.Controllers)
            {
                Controllers.Add(new LightControllerSettingViewModel(controller));
            }

            if (Controllers.Count == 0)
            {
                Controllers.Add(new LightControllerSettingViewModel(LightControllerConfig.CreateDefault(1)));
            }

            _nextControllerNumber = GetNextControllerNumber();
            SelectedController = Controllers.FirstOrDefault();
            RemoveControllerCommand.RaiseCanExecuteChanged();
        }

        private void AddController()
        {
            int controllerNumber = _nextControllerNumber++;
            var controller = new LightControllerSettingViewModel(LightControllerConfig.CreateDefault(controllerNumber));

            Controllers.Add(controller);
            SelectedController = controller;
            StatusMessage = $"컨트롤러 {controllerNumber}이(가) 추가되었습니다.";
            RemoveControllerCommand.RaiseCanExecuteChanged();
        }

        private void RemoveSelectedController()
        {
            if (SelectedController == null || Controllers.Count <= 1)
            {
                return;
            }

            int removedIndex = Controllers.IndexOf(SelectedController);
            Controllers.Remove(SelectedController);
            SelectedController = Controllers[Math.Max(0, removedIndex - 1)];
            StatusMessage = "선택한 컨트롤러를 목록에서 제거했습니다.";
            RemoveControllerCommand.RaiseCanExecuteChanged();
        }

        private void Apply()
        {
            foreach (LightControllerSettingViewModel controller in Controllers)
            {
                controller.Apply();
            }

            StatusMessage = $"마지막 적용: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _dialogService.ShowMessage("조명 컨트롤러 설정이 적용되었습니다.", "Light 설정");
        }

        private void Save()
        {
            try
            {
                _lightConfigService.Save(BuildConfig());
                StatusMessage = $"저장 완료: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                _dialogService.ShowMessage("조명 설정이 저장되었습니다.", "Light 설정");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"조명 설정 저장 중 오류가 발생했습니다.\n{ex.Message}", "Light 설정 오류");
            }
        }

        private LightConfig BuildConfig()
        {
            return new LightConfig
            {
                Controllers = Controllers.Select(controller => controller.ToModel()).ToList()
            };
        }

        private int GetNextControllerNumber()
        {
            int maxNumber = 0;

            foreach (LightControllerSettingViewModel controller in Controllers)
            {
                Match match = Regex.Match(controller.DisplayName, @"Light Controller (\d+)$");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int number) && number > maxNumber)
                {
                    maxNumber = number;
                }
            }

            return Math.Max(maxNumber + 1, Controllers.Count + 1);
        }
    }

    public class LightControllerSettingViewModel : ViewModelBase
    {
        private string _name;
        private string _comPort;
        private bool _isEnabled;

        public LightControllerSettingViewModel(LightControllerConfig config)
        {
            _name = config.Name;
            _comPort = config.ComPort;
            _isEnabled = config.Use;

            Channels = new ObservableCollection<LightChannelSettingViewModel>(
                config.Channels.Select(channel => new LightChannelSettingViewModel(channel)));
        }

        public ObservableCollection<LightChannelSettingViewModel> Channels { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string ComPort
        {
            get => _comPort;
            set
            {
                if (SetProperty(ref _comPort, value))
                {
                    OnPropertyChanged(nameof(SummaryText));
                }
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value))
                {
                    OnPropertyChanged(nameof(SummaryText));
                }
            }
        }

        public string DisplayName => string.IsNullOrWhiteSpace(Name) ? "이름 없는 컨트롤러" : Name;
        public string SummaryText => $"{(string.IsNullOrWhiteSpace(ComPort) ? "포트 미지정" : ComPort)} · {(IsEnabled ? "사용" : "미사용")}";

        public void Apply()
        {
            foreach (LightChannelSettingViewModel channel in Channels)
            {
                channel.Apply();
            }
        }

        public LightControllerConfig ToModel()
        {
            return new LightControllerConfig
            {
                Name = Name,
                ComPort = ComPort,
                Use = IsEnabled,
                Channels = Channels.Select(channel => channel.ToModel()).ToList()
            };
        }
    }

    public class LightChannelSettingViewModel : ViewModelBase
    {
        private string _channelName;
        private bool _isEnabled;
        private bool _currentEnabled;
        private int _brightness;
        private int _currentBrightness;

        public LightChannelSettingViewModel(LightChannelConfig config)
        {
            _channelName = config.Name;
            _isEnabled = config.Use;
            _brightness = NormalizeBrightness(config.Brightness);
            _currentEnabled = _isEnabled;
            _currentBrightness = _isEnabled ? _brightness : 0;
        }

        public string ChannelName
        {
            get => _channelName;
            set => SetProperty(ref _channelName, value);
        }

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

        public LightChannelConfig ToModel()
        {
            return new LightChannelConfig
            {
                Name = ChannelName,
                Use = IsEnabled,
                Brightness = Brightness
            };
        }

        private static int NormalizeBrightness(int value) => Math.Clamp(value, 0, 255);
    }
}
