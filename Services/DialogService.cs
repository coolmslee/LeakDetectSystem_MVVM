using Microsoft.Win32;
using System.Windows;

namespace LeakDetectSystem_MVVM.Services
{
    /// <summary>
    /// IDialogService의 WPF 기본 구현체.
    /// </summary>
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "알림")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string message, string title = "확인")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }

        public void ShowError(string message, string title = "오류")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

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
    }
}
