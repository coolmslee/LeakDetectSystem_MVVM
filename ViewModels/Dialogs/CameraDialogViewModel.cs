using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class CameraDialogViewModel : DialogViewModelBase
    {
        private readonly ICameraConfigService _cameraConfigService;
        private CameraConfig? _selectedCamera;
        private string _validationError = string.Empty;

        public CameraDialogViewModel()
            : this(new CameraConfigIniService())
        {
        }

        public CameraDialogViewModel(ICameraConfigService cameraConfigService)
        {
            _cameraConfigService = cameraConfigService;

            VideoFormats = new ObservableCollection<string>
            {
                "Mono8",
                "Mono10",
                "Mono12",
                "RGB8",
                "BGR8"
            };

            Cameras = new ObservableCollection<CameraConfig>(_cameraConfigService.Load());

            if (Cameras.Count == 0)
            {
                Cameras = new ObservableCollection<CameraConfig>(
                    Enumerable.Range(1, 4).Select(i => new CameraConfig { Index = i }));
            }

            SelectedCamera = Cameras.FirstOrDefault();

            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
        }

        public ObservableCollection<CameraConfig> Cameras { get; private set; }

        public ObservableCollection<string> VideoFormats { get; }

        public CameraConfig? SelectedCamera
        {
            get => _selectedCamera;
            set => SetProperty(ref _selectedCamera, value);
        }

        /// <summary>저장 유효성 검사 오류 메시지. 오류가 없으면 빈 문자열.</summary>
        public string ValidationError
        {
            get => _validationError;
            private set
            {
                if (SetProperty(ref _validationError, value))
                    OnPropertyChanged(nameof(HasValidationError));
            }
        }

        /// <summary>유효성 검사 오류가 있는지 여부.</summary>
        public bool HasValidationError => !string.IsNullOrEmpty(_validationError);

        public RelayCommand LoadCommand { get; }
        public RelayCommand SaveCommand { get; }

        private void Load()
        {
            var loaded = _cameraConfigService.Load();

            Cameras.Clear();
            foreach (var camera in loaded)
            {
                Cameras.Add(camera);
            }

            SelectedCamera = Cameras.FirstOrDefault();
            ValidationError = string.Empty;
        }

        private void Save()
        {
            string? error = ValidateCameras();
            if (error != null)
            {
                ValidationError = error;
                return;
            }

            ValidationError = string.Empty;
            _cameraConfigService.Save(Cameras);
        }

        private string? ValidateCameras()
        {
            foreach (var camera in Cameras)
            {
                if (!camera.Use)
                    continue;

                if (string.IsNullOrWhiteSpace(camera.Ip))
                    return $"{camera.Label}: IP 주소를 입력하세요.";
            }

            return null;
        }
    }
}
