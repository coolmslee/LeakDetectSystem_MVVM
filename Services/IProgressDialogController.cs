namespace LeakDetectSystem_MVVM.Services
{
    public interface IProgressDialogController : IDisposable
    {
        void UpdateMessage(string message);
        void UpdateProgress(double value);
        void SetIndeterminate(bool isIndeterminate);
        void Close();
    }
}
