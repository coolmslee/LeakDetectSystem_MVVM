using LeakDetectSystem_MVVM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class LogDialogViewModel : ViewModelBase
    {
        public ObservableCollection<string> SystemLogs { get; } = new();
        public ObservableCollection<string> AlarmLogs { get; } = new();

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public ICommand ClearCommand { get; }

        public LogDialogViewModel()
        {
            // 샘플(기존 LogEntries 대체)
            SystemLogs.Add("[INFO] System started");
            SystemLogs.Add("[INFO] Camera initialized");
            AlarmLogs.Add("[ALARM] Example alarm message");

            ClearCommand = new RelayCommand(ClearCurrentTab);
        }

        private void ClearCurrentTab()
        {
            if (SelectedTabIndex == 0) SystemLogs.Clear();
            else if (SelectedTabIndex == 1) AlarmLogs.Clear();
        }

        private sealed class RelayCommand : ICommand
        {
            private readonly Action _execute;
            public RelayCommand(Action execute) => _execute = execute;

            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute();

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}
