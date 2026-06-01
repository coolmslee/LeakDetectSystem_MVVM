using System.Windows;
using System.Windows.Threading;
using LeakDetectSystem_MVVM.Services;
using LeakDetectSystem_MVVM.ViewModels;
using LeakDetectSystem_MVVM.Views.Main;

namespace LeakDetectSystem_MVVM
{
    /// <summary>
    /// 애플리케이션 진입점.
    /// MainWindow에 MainWindowViewModel을 DataContext로 주입합니다.
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dialogService = new DialogService();
            var progressDialog = dialogService.ShowProgressDialog(
                title: "애플리케이션 시작",
                message: "애플리케이션을 초기화하는 중입니다.",
                isIndeterminate: false,
                progressValue: 10);

            await Dispatcher.Yield(DispatcherPriority.Background);

            // ViewModel을 생성하고 MainWindow에 DataContext로 주입
            // 실제 프로젝트에서는 DI 컨테이너(예: Microsoft.Extensions.DependencyInjection)를 사용하세요.
            progressDialog.UpdateMessage("메인 화면 데이터를 준비하는 중입니다.");
            progressDialog.UpdateProgress(45);
            var viewModel = new MainWindowViewModel(dialogService);

            progressDialog.UpdateMessage("메인 창을 생성하는 중입니다.");
            progressDialog.UpdateProgress(80);
            await Dispatcher.Yield(DispatcherPriority.Background);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            MainWindow = mainWindow;

            progressDialog.UpdateMessage("애플리케이션을 표시하는 중입니다.");
            progressDialog.SetIndeterminate(true);
            mainWindow.Loaded += (_, _) => progressDialog.Close();
            mainWindow.Show();
        }
    }
}
