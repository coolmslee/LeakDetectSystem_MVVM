using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public partial class PlcDialog : Window
    {
        public PlcDialog() { InitializeComponent(); }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
