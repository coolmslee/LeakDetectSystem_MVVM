using Microsoft.Win32;
using System.Windows;
using LeakDetectSystem_MVVM.Views.Dialogs;

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

        public void ShowPlcDialog() => new PlcDialog { Owner = GetMainWindow() }.Show();
        public void ShowModelDialog() => new ModelDialog { Owner = GetMainWindow() }.Show();
        public void ShowCameraDialog() => new CameraDialog { Owner = GetMainWindow() }.Show();
        public void ShowGrabDialog() => new GrabDialog { Owner = GetMainWindow() }.Show();
        public void ShowLightDialog() => new LightDialog { Owner = GetMainWindow() }.Show();
        public void ShowLogDialog() => new LogDialog { Owner = GetMainWindow() }.Show();

        private static Window? GetMainWindow() => Application.Current?.MainWindow;
    }
}
