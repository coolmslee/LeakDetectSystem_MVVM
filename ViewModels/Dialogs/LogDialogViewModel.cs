using LeakDetectSystem_MVVM.ViewModels.Base;
using System.Collections.ObjectModel;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class LogDialogViewModel : ViewModelBase
    {
        public ObservableCollection<string> LogEntries { get; } = new()
        {
            "[INFO] System started",
            "[INFO] Camera initialized",
        };
    }
}
