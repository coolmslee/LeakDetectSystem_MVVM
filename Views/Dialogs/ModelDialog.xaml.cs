using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public partial class ModelDialog : Window
    {
        public ModelDialog() { InitializeComponent(); }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
