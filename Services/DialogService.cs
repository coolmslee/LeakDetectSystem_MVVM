using Microsoft.Win32;
using System.Windows;
using LeakDetectSystem_MVVM.Views.Dialogs;
using LeakDetectSystem_MVVM.ViewModels.Dialogs;

namespace LeakDetectSystem_MVVM.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "알림")
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public bool ShowConfirmation(string message, string title = "확인")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }

        public void ShowError(string message, string title = "오류")
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

        public string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*")
        {
            var dialog = new OpenFileDialog { Filter = filter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*")
        {
            var dialog = new SaveFileDialog { Filter = filter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public void ShowPlcDialog()
        {
            var dialog = new PlcDialog
            {
                Owner = GetMainWindow(),
                DataContext = new PlcDialogViewModel()
            };

            dialog.Show();
        }
        public void ShowModelDialog()
        {
            var dialog = new ModelDialog
            {
                Owner = GetMainWindow(),
                DataContext = new ModelDialogViewModel()
            };

            dialog.Show();
        }

        public void ShowCameraDialog()
        {
            var dialog = new CameraDialog
            {
                Owner = GetMainWindow(),
                DataContext = new CameraDialogViewModel()
            };

            dialog.Show();
        }

        public void ShowGrabDialog()
        {
            var dialog = new GrabDialog
            {
                Owner = GetMainWindow(),
                DataContext = new GrabDialogViewModel()
            };

            dialog.Show();
        }

        public void ShowLightDialog()
        {
            var dialog = new LightDialog
            {
                Owner = GetMainWindow(),
                DataContext = new LightDialogViewModel()
            };

            dialog.Show();
        }

        public void ShowLogDialog()
        {
            var dialog = new LogDialog
            {
                Owner = GetMainWindow(),
                DataContext = new LogDialogViewModel()
            };
            
            dialog.Show();
        }
        private static Window? GetMainWindow() => Application.Current?.MainWindow;
    }
}
