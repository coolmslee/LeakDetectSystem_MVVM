using System.Windows;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ViewModel을 생성하고 MainWindow에 DataContext로 주입
            // 실제 프로젝트에서는 DI 컨테이너(예: Microsoft.Extensions.DependencyInjection)를 사용하세요.
            var viewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }
    }
}
