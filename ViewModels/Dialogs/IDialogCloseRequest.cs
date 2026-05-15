using System;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public interface IDialogCloseRequest
    {
        event EventHandler? CloseRequested;
    }
}
