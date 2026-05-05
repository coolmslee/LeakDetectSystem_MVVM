using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public partial class GrabDialog : Window
    {
        public GrabDialog() { InitializeComponent(); }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
