using System;
using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Main
{
    /// <summary>
    /// MainWindow의 코드-비하인드.
    /// DataContext 설정은 App.xaml.cs에서 처리하므로 여기서는 InitializeComponent()만 호출합니다.
    /// 창이 닫힐 때 DataContext가 IDisposable이면 Dispose를 호출합니다.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (DataContext as IDisposable)?.Dispose();
        }
    }
}
