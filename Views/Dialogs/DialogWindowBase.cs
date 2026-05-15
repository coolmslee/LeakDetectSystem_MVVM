using LeakDetectSystem_MVVM.ViewModels.Dialogs;
using System;
using System.Windows;

namespace LeakDetectSystem_MVVM.Views.Dialogs
{
    /// <summary>
    /// 모든 Dialog 창의 공통 기반 클래스.
    /// DataContext가 IDialogCloseRequest를 구현하면 CloseRequested 이벤트로 창을 닫습니다.
    /// </summary>
    public class DialogWindowBase : Window
    {
        public DialogWindowBase()
        {
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
