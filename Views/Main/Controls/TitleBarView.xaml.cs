using System.Windows.Controls;

namespace LeakDetectSystem_MVVM.Views.Main.Controls
{
    /// <summary>
    /// TitleBarView의 코드-비하인드.
    /// DataContext는 MainWindow에서 {Binding TitleBar}로 연결됩니다.
    /// </summary>
    public partial class TitleBarView : UserControl
    {
        public TitleBarView()
        {
            InitializeComponent();
        }
    }
}