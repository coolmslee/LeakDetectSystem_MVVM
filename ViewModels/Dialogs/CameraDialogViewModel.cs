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
        }

        private void Save()
        {
            _cameraConfigService.Save(Cameras);
        }
    }
}
