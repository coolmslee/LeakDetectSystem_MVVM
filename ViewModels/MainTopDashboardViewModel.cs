using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    public class MainTopDashboardViewModel : ViewModelBase
    {
        private readonly ObservableCollection<CameraConfig> _cameras;
        private readonly IDialogService? _dialogService;

        // ─── 검사 PASS (PC) ───
        private bool _isInspectPass;
        public bool IsInspectPass { get => _isInspectPass; set => SetProperty(ref _isInspectPass, value); }

        // ─── 물류 PASS (PLC) ───
        private bool _isPlcPass;
        public bool IsPlcPass { get => _isPlcPass; set => SetProperty(ref _isPlcPass, value); }

        // ─── 판정 결과 ───
        private string _resultText = "--";
        public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

        // ─── 수량/수율 ───
        private int _totalCount;
        public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); UpdateYield(); } }

        private int _okCount;
        public int OkCount { get => _okCount; set { SetProperty(ref _okCount, value); UpdateYield(); } }

        private int _ngCount;
        public int NgCount { get => _ngCount; set => SetProperty(ref _ngCount, value); }

        private string _yield = "0.0%";
        public string Yield { get => _yield; private set => SetProperty(ref _yield, value); }

        // ─── CAM 선택 (설정 완료된 카메라만, 1개 콤보) ───
        public ObservableCollection<string> AvailableCameras { get; } = new();

        private string _selectedCamera = string.Empty;
        public string SelectedCamera { get => _selectedCamera; set => SetProperty(ref _selectedCamera, value); }

        // ─── 연결 상태 ───
        private bool _isLightConnected;
        public bool IsLightConnected { get => _isLightConnected; set => SetProperty(ref _isLightConnected, value); }

        private bool _isPlcConnected;
        public bool IsPlcConnected { get => _isPlcConnected; set => SetProperty(ref _isPlcConnected, value); }

        // ─── SEQ ───
        private string _seqNo = "-";
        public string SeqNo { get => _seqNo; set => SetProperty(ref _seqNo, value); }

        // ─── 자동시작 / 자동정지 ───
        private bool _isAutoStart;
        public bool IsAutoStart { get => _isAutoStart; set => SetProperty(ref _isAutoStart, value); }

        private bool _isAutoStop;
        public bool IsAutoStop { get => _isAutoStop; set => SetProperty(ref _isAutoStop, value); }

        // ─── Commands ───
        public RelayCommand ResetCountCommand { get; }
        public RelayCommand ResetVppCommand { get; }
        public RelayCommand ManualInspectCommand { get; }
        public RelayCommand AutoStartCommand { get; }
        public RelayCommand AutoStopCommand { get; }

        // ─── 기본 생성자 (디자인 타임 / 테스트용) ───
        public MainTopDashboardViewModel() : this(new ObservableCollection<CameraConfig>
        {
            new CameraConfig { Index = 1, Use = true, Ip = "192.168.0.1" },
            new CameraConfig { Index = 2, Use = true, Ip = "192.168.0.2" },
            new CameraConfig { Index = 3 },
            new CameraConfig { Index = 4 },
        }) { }

        public MainTopDashboardViewModel(ObservableCollection<CameraConfig> cameras)
            : this(cameras, null) { }

        public MainTopDashboardViewModel(ObservableCollection<CameraConfig> cameras, IDialogService? dialogService)
        {
            _cameras       = cameras;
            _dialogService = dialogService;

            ResetCountCommand    = new RelayCommand(ResetCount);
            ResetVppCommand      = new RelayCommand(ResetVpp);
            ManualInspectCommand = new RelayCommand(ManualInspect);
            AutoStartCommand     = new RelayCommand(OnAutoStart);
            AutoStopCommand      = new RelayCommand(OnAutoStop);

            foreach (var cam in _cameras)
                cam.PropertyChanged += OnCameraPropertyChanged;
            _cameras.CollectionChanged += OnCamerasCollectionChanged;

            RefreshAvailableCameras();
        }

        private void OnCameraPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CameraConfig.IsConfigured))
                RefreshAvailableCameras();
        }

        private void OnCamerasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CameraConfig cam in e.NewItems)
                    cam.PropertyChanged += OnCameraPropertyChanged;
            if (e.OldItems != null)
                foreach (CameraConfig cam in e.OldItems)
                    cam.PropertyChanged -= OnCameraPropertyChanged;
            RefreshAvailableCameras();
        }

        private void RefreshAvailableCameras()
        {
            var current = SelectedCamera;
            AvailableCameras.Clear();
            foreach (var cam in _cameras.Where(c => c.IsConfigured))
                AvailableCameras.Add(cam.Label);
            SelectedCamera = AvailableCameras.Contains(current) ? current : (AvailableCameras.FirstOrDefault() ?? string.Empty);
        }

        private void UpdateYield()
        {
            Yield = TotalCount > 0 ? $"{OkCount * 100.0 / TotalCount:F1}%" : "0.0%";
        }

        private void ResetCount()
        {
            TotalCount = 0;
            OkCount    = 0;
            NgCount    = 0;
        }

        private void ResetVpp() { }
        private void ManualInspect() { }

        private void OnAutoStart()
        {
            var reasons = new List<string>();
            if (!IsPlcConnected)   reasons.Add("PLC가 연결되어 있지 않습니다.");
            if (!IsLightConnected) reasons.Add("조명 컨트롤러가 연결되어 있지 않습니다.");

            if (reasons.Count > 0)
            {
                var message = "자동시작이 불가합니다:\n\n" + string.Join("\n", reasons);
                if (_dialogService != null)
                    _dialogService.ShowError(message, "자동시작 오류");
                else
                    System.Windows.MessageBox.Show(message, "자동시작 오류",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            IsAutoStart = true;
            IsAutoStop  = false;
        }

        private void OnAutoStop()
        {
            IsAutoStart = false;
            IsAutoStop  = true;
        }
    }
}
