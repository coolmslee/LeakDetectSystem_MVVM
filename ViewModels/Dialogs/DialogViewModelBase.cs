using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.ViewModels.Base;
using System;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public abstract class DialogViewModelBase : ViewModelBase, IDialogCloseRequest
    {
        public RelayCommand CloseCommand { get; }

        public event EventHandler? CloseRequested;

        protected DialogViewModelBase()
        {
            CloseCommand = new RelayCommand(RequestClose);
        }

        protected void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
