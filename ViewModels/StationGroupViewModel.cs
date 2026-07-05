using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class StationGroupViewModel : ViewModelBase, IDisposable
    {
        private readonly ObservableCollection<CameraConfig> _cameras;
        private StationCardViewModel? _selectedStation;
        private readonly List<ICognexCameraService> _cameraServices = new List<ICognexCameraService>();
        private bool _disposed;

        public ObservableCollection<StationCardViewModel> Stations { get; } = new ObservableCollection<StationCardViewModel>();

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
            // 기존 서비스 및 VM 정리
            foreach (var vm in Stations)
                vm.Dispose();
            Stations.Clear();

            foreach (var svc in _cameraServices)
                svc.Dispose();
            _cameraServices.Clear();

            foreach (var cam in _cameras.Where(c => c.IsConfigured))
            {
                // 카메라 서비스 생성 및 연결 시도
                var service = new CognexCameraService();
                try
                {
                    service.Connect(cam);
                }
                catch (Exception ex)
                {
                    // 연결 실패 시 로그 출력 후 미연결 상태로 계속
                    System.Diagnostics.Debug.WriteLine(
                        $"[StationGroupViewModel] CAM{cam.Index} ({cam.Ip}) 연결 실패: {ex.Message}");
                }
                _cameraServices.Add(service);

                var stationVm = new StationCardViewModel
                {
                    StationId = cam.Index,
                    StationName = $"ST{cam.Index}",
                    PressureValue = 0,
                    Threshold = 110.0,
                    ResultState = StationResultState.Unknown,
                    IsLive = true,
                    CameraService = service,
                };
                Stations.Add(stationVm);
            }

            SelectedStation = Stations.FirstOrDefault();
            OnPropertyChanged(nameof(GridColumns));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var vm in Stations)
                vm.Dispose();

            foreach (var svc in _cameraServices)
                svc.Dispose();
            _cameraServices.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
