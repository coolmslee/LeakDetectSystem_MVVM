using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// Main 탭 상단 대시보드 ViewModel.
    /// 원본 LeakDetectionSystem SearchPanel 기준의 상태/제어 항목을 제공합니다.
    /// 카메라 선택(최대 4개, 현재 2개 노출), 검사/물류 PASS, 판정결과,
    /// 수량/수율, 초기화 버튼, 연결상태(LIGHT/PLC), SEQ, 자동시작/자동정지를 관리합니다.
    /// </summary>
    public class MainTopDashboardViewModel : ViewModelBase
    {
        // ─── 검사 PASS (PC) ───

        private bool _isInspectPass;

        /// <summary>공병검사(PC) 검사 PASS 여부</summary>
        public bool IsInspectPass
        {
            get => _isInspectPass;
            set => SetProperty(ref _isInspectPass, value);
        }

        // ─── 물류 PASS (PLC) ───

        private bool _isPlcPass;

        /// <summary>물류PASS(PLC) 신호 수신 여부 (Ellipse 표시)</summary>
        public bool IsPlcPass
        {
            get => _isPlcPass;
            set => SetProperty(ref _isPlcPass, value);
        }

        // ─── 판정 결과 ───

        private string _resultText = "--";

        /// <summary>판정결과 문자열 (예: "OK", "NG", "--")</summary>
        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        // ─── 수량/수율 ───

        private int _totalCount;

        /// <summary>TOTAL 검사 수량</summary>
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                SetProperty(ref _totalCount, value);
                UpdateYield();
            }
        }

        private int _okCount;

        /// <summary>OK 수량</summary>
        public int OkCount
        {
            get => _okCount;
            set
            {
                SetProperty(ref _okCount, value);
                UpdateYield();
            }
        }

        private int _ngCount;

        /// <summary>NG 수량</summary>
        public int NgCount
        {
            get => _ngCount;
            set => SetProperty(ref _ngCount, value);
        }

        private string _yield = "0.0%";

        /// <summary>수율 (OkCount/TotalCount × 100, 자동 갱신)</summary>
        public string Yield
        {
            get => _yield;
            private set => SetProperty(ref _yield, value);
        }

        // ─── CAM 선택 (최대 4개, 현재 ComboBox 2개 노출) ───

        /// <summary>카메라 옵션 목록 (CAM1~CAM4, 확장 가능)</summary>
        public ObservableCollection<string> CameraOptions { get; } =
            new() { "CAM1", "CAM2", "CAM3", "CAM4" };

        private string _selectedCamera1 = "CAM1";

        /// <summary>CAM 선택 1</summary>
        public string SelectedCamera1
        {
            get => _selectedCamera1;
            set => SetProperty(ref _selectedCamera1, value);
        }

        private string _selectedCamera2 = "CAM2";

        /// <summary>CAM 선택 2</summary>
        public string SelectedCamera2
        {
            get => _selectedCamera2;
            set => SetProperty(ref _selectedCamera2, value);
        }

        // ─── 연결 상태 (LIGHT / PLC) ───

        private bool _isLightConnected;

        /// <summary>LIGHT 연결 상태</summary>
        public bool IsLightConnected
        {
            get => _isLightConnected;
            set => SetProperty(ref _isLightConnected, value);
        }

        private bool _isPlcConnected;

        /// <summary>PLC 연결 상태</summary>
        public bool IsPlcConnected
        {
            get => _isPlcConnected;
            set => SetProperty(ref _isPlcConnected, value);
        }

        // ─── SEQ ───

        private string _seqNo = "-";

        /// <summary>SEQ 번호 표시</summary>
        public string SeqNo
        {
            get => _seqNo;
            set => SetProperty(ref _seqNo, value);
        }

        // ─── 자동시작 / 자동정지 ───

        private bool _isAutoStart;

        /// <summary>자동시작 토글 상태</summary>
        public bool IsAutoStart
        {
            get => _isAutoStart;
            set => SetProperty(ref _isAutoStart, value);
        }

        private bool _isAutoStop;

        /// <summary>자동정지 토글 상태</summary>
        public bool IsAutoStop
        {
            get => _isAutoStop;
            set => SetProperty(ref _isAutoStop, value);
        }

        // ─── Commands ───

        /// <summary>수량 초기화</summary>
        public RelayCommand ResetCountCommand { get; }

        /// <summary>VPP 초기화</summary>
        public RelayCommand ResetVppCommand { get; }

        /// <summary>수동 검사</summary>
        public RelayCommand ManualInspectCommand { get; }

        // ─── 생성자 ───

        public MainTopDashboardViewModel()
        {
            ResetCountCommand    = new RelayCommand(ResetCount);
            ResetVppCommand      = new RelayCommand(ResetVpp);
            ManualInspectCommand = new RelayCommand(ManualInspect);
        }

        // ─── 내부 헬퍼 ───

        private void UpdateYield()
        {
            Yield = TotalCount > 0
                ? $"{OkCount * 100.0 / TotalCount:F1}%"
                : "0.0%";
        }

        private void ResetCount()
        {
            TotalCount = 0;
            OkCount    = 0;
            NgCount    = 0;
            // Yield는 TotalCount/OkCount setter 내 UpdateYield() 호출로 자동 갱신됩니다.
        }

        private void ResetVpp()
        {
            // VPP 초기화 – 실제 서비스 레이어 연결 시 구현
        }

        private void ManualInspect()
        {
            // 수동 검사 – 실제 서비스 레이어 연결 시 구현
        }
    }
}
