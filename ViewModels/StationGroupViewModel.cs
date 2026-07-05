using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class StationGroupViewModel : ViewModelBase
    {
        private readonly ObservableCollection<CameraConfig> _cameras;
        private StationCardViewModel? _selectedStation;

        public ObservableCollection<StationCardViewModel> Stations { get; } = new();

        public StationCardViewModel? SelectedStation
        {
            get => _selectedStation;
            set => SetProperty(ref _selectedStation, value);
        }

        public RelayCommand<StationCardViewModel> SelectStationCommand { get; }

        /// <summary>Number of columns for the UniformGrid (1 for <=1 station, 2 for 2+ stations)</summary>
        public int GridColumns => Stations.Count <= 1 ? 1 : 2;

        public StationGroupViewModel() : this(new ObservableCollection<CameraConfig>
        {
            new CameraConfig { Index = 1, Use = true, Ip = "192.168.0.1" },
            new CameraConfig { Index = 2, Use = true, Ip = "192.168.0.2" },
            new CameraConfig { Index = 3 },
            new CameraConfig { Index = 4 },
        }) { }

        public StationGroupViewModel(ObservableCollection<CameraConfig> cameras)
        {
            _cameras = cameras;
            SelectStationCommand = new RelayCommand<StationCardViewModel>(s => SelectedStation = s);

            foreach (var cam in _cameras)
                cam.PropertyChanged += OnCameraPropertyChanged;

            _cameras.CollectionChanged += OnCamerasCollectionChanged;

            RebuildStations();
        }

        private void OnCameraPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CameraConfig.IsConfigured))
                RebuildStations();
        }

        private void OnCamerasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CameraConfig cam in e.NewItems)
                    cam.PropertyChanged += OnCameraPropertyChanged;
            if (e.OldItems != null)
                foreach (CameraConfig cam in e.OldItems)
                    cam.PropertyChanged -= OnCameraPropertyChanged;
            RebuildStations();
        }

        private void RebuildStations()
        {
            Stations.Clear();
            foreach (var cam in _cameras.Where(c => c.IsConfigured))
            {
                Stations.Add(new StationCardViewModel
                {
                    StationId = cam.Index,
                    StationName = $"ST{cam.Index}",
                    PressureValue = 0,
                    Threshold = 110.0,
                    ResultState = StationResultState.Unknown,
                    IsLive = true,
                });
            }
            OnPropertyChanged(nameof(GridColumns));
        }
    }
}
