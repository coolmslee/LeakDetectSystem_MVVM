using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using System;
using System.Collections.ObjectModel;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class GrabDialogViewModel : DialogViewModelBase
    {
        private readonly IGrabConfigService _grabConfigService;
        private readonly IDialogService _dialogService;
        private int _interval = 100;
        private bool _imageSave = true;
        private string _selectedImageExtension = "BMP";
        private int _hddSettingSpace = 10;
        private string _statusMessage = string.Empty;

        public GrabDialogViewModel()
            : this(new GrabConfigIniService(), new DialogService())
        {
        }

        public GrabDialogViewModel(IGrabConfigService grabConfigService, IDialogService dialogService)
        {
            _grabConfigService = grabConfigService;
            _dialogService = dialogService;

            ImageExtensions = new ObservableCollection<string> { "BMP", "JPEG" };

            SaveGrabSettingsCommand = new RelayCommand(SaveGrabSettings);
            SaveHddSettingsCommand = new RelayCommand(SaveHddSettings);

            LoadFromIni();
        }

        public int Interval
        {
            get => _interval;
            set => SetProperty(ref _interval, value);
        }

        public bool ImageSave
        {
            get => _imageSave;
            set => SetProperty(ref _imageSave, value);
        }

        public ObservableCollection<string> ImageExtensions { get; }

        public string SelectedImageExtension
        {
            get => _selectedImageExtension;
            set => SetProperty(ref _selectedImageExtension, NormalizeImageExtension(value));
        }

        public int HddSettingSpace
        {
            get => _hddSettingSpace;
            set => SetProperty(ref _hddSettingSpace, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public RelayCommand SaveGrabSettingsCommand { get; }
        public RelayCommand SaveHddSettingsCommand { get; }

        private void LoadFromIni()
        {
            try
            {
                GrabConfig config = _grabConfigService.Load();
                Interval = config.Interval;
                ImageSave = config.ImageSave;
                SelectedImageExtension = config.ImageExtension;
                HddSettingSpace = config.HddSettingSpace;
                StatusMessage = "설정을 불러왔습니다.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"설정 로드 실패: {ex.Message}";
            }
        }

        private void SaveGrabSettings()
        {
            bool adjusted = NormalizeInputsForSave();
            SaveConfiguration();
            StatusMessage = adjusted
                ? $"Grab 설정 저장/적용 완료(최소값 보정): {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                : $"Grab 설정 저장/적용 완료: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _dialogService.ShowMessage("Grab 설정이 저장되고 즉시 적용되었습니다.", "Grab 설정");
        }

        private void SaveHddSettings()
        {
            bool adjusted = NormalizeInputsForSave();
            SaveConfiguration();
            StatusMessage = adjusted
                ? $"HDD 설정 저장/적용 완료(최소값 보정): {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                : $"HDD 설정 저장/적용 완료: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _dialogService.ShowMessage("HDD 설정이 저장되고 즉시 적용되었습니다.", "HDD Setting");
        }

        private void SaveConfiguration()
        {
            _grabConfigService.Save(new GrabConfig
            {
                Interval = Interval,
                ImageSave = ImageSave,
                ImageExtension = SelectedImageExtension,
                HddSettingSpace = HddSettingSpace
            });
        }

        private static string NormalizeImageExtension(string? extension)
        {
            return string.Equals(extension?.Trim(), "JPEG", StringComparison.OrdinalIgnoreCase)
                ? "JPEG"
                : "BMP";
        }

        private bool NormalizeInputsForSave()
        {
            bool adjusted = false;

            if (Interval < 1)
            {
                Interval = 1;
                adjusted = true;
            }

            if (HddSettingSpace < 1)
            {
                HddSettingSpace = 1;
                adjusted = true;
            }

            string normalizedExtension = NormalizeImageExtension(SelectedImageExtension);
            if (!string.Equals(normalizedExtension, SelectedImageExtension, StringComparison.Ordinal))
            {
                SelectedImageExtension = normalizedExtension;
                adjusted = true;
            }

            return adjusted;
        }
    }
}
