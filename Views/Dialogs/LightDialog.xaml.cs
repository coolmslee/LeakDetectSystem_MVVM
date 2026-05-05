using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public partial class LightDialog : Window
    {
        public LightDialog() { InitializeComponent(); }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
