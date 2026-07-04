using System.Collections.ObjectModel;
using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "알림");
        bool ShowConfirmation(string message, string title = "확인");
        void ShowError(string message, string title = "오류");
        void ShowMessageDialog(MessageDialogRequest request, Action<MessageDialogResult, string?>? onCompleted = null);
        string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*");
        string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*");

        string? ShowInputDialog(string message, string title = "입력", string? defaultValue = null);

        void ShowPlcDialog(Action? onSaved = null);
        void ShowModelDialog();
        void ShowCameraDialog(ObservableCollection<CameraConfig>? cameras = null);
        void ShowGrabDialog();
        void ShowLightDialog(Action? onSaved = null);
        void ShowLogDialog();
        IProgressDialogController ShowProgressDialog(
            string title = "진행 상태",
            string message = "작업을 준비하는 중입니다.",
            bool isIndeterminate = true,
            double progressValue = 0);
    }
}
