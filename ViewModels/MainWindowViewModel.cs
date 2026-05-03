using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// MainWindow의 ViewModel.
    /// 탭 목록을 관리하고, 애플리케이션 레벨의 명령(종료, 정보 등)을 제공합니다.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private object? _selectedTab;
        private string _title = "누설 감지 시스템";
        private string _footerStatus = "준비";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string FooterStatus
        {
            get => _footerStatus;
            set => SetProperty(ref _footerStatus, value);
        }

        public object? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        // 탭 ViewModels
        public MainTabViewModel MainTab { get; } = new();
        public SettingTabViewModel SettingTab { get; } = new();

        // TitleBar ViewModel
        public TitleBarViewModel TitleBar { get; }

        // Commands
        public RelayCommand ExitCommand { get; }
        public RelayCommand ShowAboutCommand { get; }
        public RelayCommand<string> NavigateCommand { get; }

        public MainWindowViewModel() : this(new DialogService()) { }

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            // TitleBar ViewModel – FooterStatus 콜백으로 연결
            TitleBar = new TitleBarViewModel(msg => FooterStatus = msg);

            ExitCommand = new RelayCommand(() => System.Windows.Application.Current.Shutdown());
            ShowAboutCommand = new RelayCommand(ShowAbout);
            NavigateCommand = new RelayCommand<string>(NavigateTo);

            // 기본 탭 선택
            SelectedTab = MainTab;
        }

        private void ShowAbout()
        {
            _dialogService.ShowMessage(
                "누설 감지 시스템 MVVM 예시\n버전 1.0.0\n\nWPF + MVVM 패턴으로 구현된 샘플 프로젝트입니다.",
                "프로그램 정보");
        }

        private void NavigateTo(string? viewName)
        {
            SelectedTab = viewName switch
            {
                "main"    => MainTab,
                "setting" => SettingTab,
                _         => SelectedTab
            };
            FooterStatus = $"탭 이동: {viewName}";
        }
    }
}
