namespace LeakDetectSystem_MVVM.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "알림");
        bool ShowConfirmation(string message, string title = "확인");
        void ShowError(string message, string title = "오류");
        string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*");
        string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*");

        void ShowPlcDialog();
        void ShowModelDialog();
        void ShowCameraDialog();
        void ShowGrabDialog();
        void ShowLightDialog();
        void ShowLogDialog();
    }
}
