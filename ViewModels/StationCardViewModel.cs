using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 개별 스테이션 카드(ST1~ST4) 에 대한 ViewModel.
    /// StationCardView.xaml 에 DataContext 로 바인딩됩니다.
    ///
    /// 원본(LeakDetectSystem) 기준 매핑:
    ///   - StationName     ← ST1/ST2/ST3/ST4 레이블
    ///   - IsLive          ← LIVE 버튼 활성 상태
    ///   - IsFitMode       ← FIT 버튼 활성 상태
    ///   - IsOneToOne      ← 1:1 버튼 활성 상태
    ///   - ResultState     ← OK/NG 판정 결과
    ///   - PressureValue   ← 압력 측정값
    ///   - Threshold       ← 누설 판단 임계값
    /// </summary>
    public class StationCardViewModel : ViewModelBase
    {
        private int _stationId;
        private string _stationName = string.Empty;
        private bool _isLive;
        private bool _isFitMode;
        private bool _isOneToOne;
        private StationResultState _resultState;
        private double _pressureValue;
        private double _threshold;
        private bool _isMonitoring;

        // ── 기본 식별 속성 ──────────────────────────────────────

        public int StationId
        {
            get => _stationId;
            set => SetProperty(ref _stationId, value);
        }

        public string StationName
        {
            get => _stationName;
            set => SetProperty(ref _stationName, value);
        }

        // ── 표시 모드 속성 (CogDisplay 조작) ───────────────────
        // 원본: CogDisplay 의 LIVE/FIT/1:1 버튼에 대응

        /// <summary>라이브 영상 표시 모드 활성 여부</summary>
        public bool IsLive
        {
            get => _isLive;
            set => SetProperty(ref _isLive, value);
        }

        /// <summary>FIT(화면 맞춤) 표시 모드 활성 여부</summary>
        public bool IsFitMode
        {
            get => _isFitMode;
            set => SetProperty(ref _isFitMode, value);
        }

        /// <summary>1:1 픽셀 표시 모드 활성 여부</summary>
        public bool IsOneToOne
        {
            get => _isOneToOne;
            set => SetProperty(ref _isOneToOne, value);
        }

        // ── 검사 결과 속성 ──────────────────────────────────────

        /// <summary>OK / NG / Unknown 판정 결과</summary>
        public StationResultState ResultState
        {
            get => _resultState;
            set => SetProperty(ref _resultState, value, () =>
            {
                OnPropertyChanged(nameof(IsOK));
                OnPropertyChanged(nameof(IsNG));
                OnPropertyChanged(nameof(ResultText));
            });
        }

        public bool IsOK => ResultState == StationResultState.OK;
        public bool IsNG => ResultState == StationResultState.NG;

        /// <summary>결과 텍스트 표시용 ("OK" / "NG" / "---")</summary>
        public string ResultText => ResultState switch
        {
            StationResultState.OK => "OK",
            StationResultState.NG => "NG",
            _ => "---",
        };

        // ── 측정값 속성 ─────────────────────────────────────────

        /// <summary>현재 압력 측정값 (kPa)</summary>
        public double PressureValue
        {
            get => _pressureValue;
            set => SetProperty(ref _pressureValue, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        /// <summary>누설 판단 임계값 (kPa)</summary>
        public double Threshold
        {
            get => _threshold;
            set => SetProperty(ref _threshold, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        /// <summary>측정값이 임계값을 초과하는지 여부</summary>
        public bool IsAboveThreshold => PressureValue > Threshold;

        /// <summary>현재 모니터링 동작 중 여부 (전체 토글 연동)</summary>
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        // ── 표시 모드 Commands ──────────────────────────────────
        // 원본: LIVE / FIT / 1:1 버튼 클릭 이벤트에 대응

        /// <summary>LIVE 모드로 전환</summary>
        public RelayCommand LiveCommand { get; }

        /// <summary>1:1 픽셀 모드로 전환</summary>
        public RelayCommand OneToOneCommand { get; }

        /// <summary>FIT(화면 맞춤) 모드 토글</summary>
        public RelayCommand FitCommand { get; }

        public StationCardViewModel()
        {
            LiveCommand = new RelayCommand(() =>
            {
                IsLive = true;
                IsFitMode = false;
                IsOneToOne = false;
            });

            OneToOneCommand = new RelayCommand(() =>
            {
                IsOneToOne = true;
                IsLive = false;
                IsFitMode = false;
            });

            FitCommand = new RelayCommand(() =>
            {
                IsFitMode = true;
                IsLive = false;
                IsOneToOne = false;
            });
        }
    }
}
