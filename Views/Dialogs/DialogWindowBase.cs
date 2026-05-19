using LeakDetectSystem_MVVM.ViewModels.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    public class DialogWindowBase : Window
    {
        public DialogWindowBase()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IDialogCloseRequest oldVm)
            {
                oldVm.CloseRequested -= OnCloseRequested;
            }

            if (e.NewValue is IDialogCloseRequest newVm)
            {
                newVm.CloseRequested += OnCloseRequested;
            }
        }
        private void OnCloseRequested(object? sender, EventArgs e) => Close();
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is IDialogCloseRequest vm)
            {
                vm.CloseRequested -= OnCloseRequested;
            }

            DataContextChanged -= OnDataContextChanged;
            base.OnClosed(e);
        }
    }
}