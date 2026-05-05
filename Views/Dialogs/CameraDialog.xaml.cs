using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public partial class CameraDialog : Window
    {
        public CameraDialog() { InitializeComponent(); }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
