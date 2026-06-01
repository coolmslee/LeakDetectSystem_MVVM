using LeakDetectSystem_MVVM.ViewModels.Dialogs;
using LeakDetectSystem_MVVM.Views.Dialogs;
using Microsoft.Win32;
using System.Windows;

namespace LeakDetectSystem_MVVM.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "알림")
        {
            ShowMessageDialog(new MessageDialogRequest
            {
                Title = title,
                Header = title,
                Message = message,
                Buttons = MessageDialogButtons.OK,
                DialogType = MessageDialogType.Info
            });
        }

        public bool ShowConfirmation(string message, string title = "확인")
        {
            MessageDialogResult result = MessageDialogResult.None;
            ShowMessageDialogInternal(
                new MessageDialogRequest
                {
                    Title = title,
                    Header = title,
                    Message = message,
                    Buttons = MessageDialogButtons.OKCancel,
                    DialogType = MessageDialogType.Question
                },
                isModal: true,
                (dialogResult, _) => result = dialogResult);

            return result == MessageDialogResult.OK;
        }

        public void ShowError(string message, string title = "오류")
        {
            ShowMessageDialog(new MessageDialogRequest
            {
                Title = title,
                Header = title,
                Message = message,
                Buttons = MessageDialogButtons.OK,
                DialogType = MessageDialogType.Error
            });
        }

        public void ShowMessageDialog(MessageDialogRequest request, Action<MessageDialogResult, string?>? onCompleted = null)
            => ShowMessageDialogInternal(request, isModal: false, onCompleted);

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

        public void ShowPlcDialog()
        {
            var dialog = new PlcDialog
            {
                Owner = GetMainWindow(),
                DataContext = new PlcDialogViewModel()
            };

            dialog.Show();
        }
        public void ShowModelDialog()
        {
            var dialog = new ModelDialog
            {
                Owner = GetMainWindow(),
                DataContext = new ModelDialogViewModel()
            };

            dialog.ShowDialog();
        }

        public void ShowCameraDialog()
        {
            var dialog = new CameraDialog
            {
                Owner = GetMainWindow(),
                DataContext = new CameraDialogViewModel()
            };

            dialog.ShowDialog();
        }

        public void ShowGrabDialog()
        {
            var dialog = new GrabDialog
            {
                Owner = GetMainWindow(),
                DataContext = new GrabDialogViewModel()
            };

            dialog.ShowDialog();
        }

        public void ShowLightDialog()
        {
            var dialog = new LightDialog
            {
                Owner = GetMainWindow(),
                DataContext = new LightDialogViewModel()
            };

            dialog.ShowDialog();
        }

        public void ShowLogDialog()
        {
            var dialog = new LogDialog
            {
                Owner = GetMainWindow(),
                DataContext = new LogDialogViewModel()
            };
            
            dialog.Show();
        }

        public IProgressDialogController ShowProgressDialog(
            string title = "진행 상태",
            string message = "작업을 준비하는 중입니다.",
            bool isIndeterminate = true,
            double progressValue = 0)
        {
            ProgressDialog? dialog = null;
            ProgressDialogViewModel? viewModel = null;

            InvokeOnUiThread(() =>
            {
                viewModel = new ProgressDialogViewModel(title, message, progressValue, isIndeterminate);
                dialog = new ProgressDialog
                {
                    Owner = GetMainWindow(),
                    DataContext = viewModel
                };

                dialog.Show();
            });

            return new ProgressDialogController(dialog!, viewModel!);
        }

        private static Window? GetMainWindow() => Application.Current?.MainWindow;

        private void ShowMessageDialogInternal(
            MessageDialogRequest request,
            bool isModal,
            Action<MessageDialogResult, string?>? onCompleted)
        {
            ArgumentNullException.ThrowIfNull(request);

            InvokeOnUiThread(() =>
            {
                var viewModel = new MessageDialogViewModel(request);
                var dialog = new MessageDialog
                {
                    Owner = GetMainWindow(),
                    DataContext = viewModel
                };

                dialog.Closed += (_, _) => onCompleted?.Invoke(viewModel.Result, viewModel.InputText);

                if (isModal)
                    dialog.ShowDialog();
                else
                    dialog.Show();
            });
        }

        private static void InvokeOnUiThread(Action action)
        {
            if (Application.Current?.Dispatcher is not { } dispatcher || dispatcher.CheckAccess())
            {
                action();
                return;
            }

            dispatcher.Invoke(action);
        }

        private sealed class ProgressDialogController : IProgressDialogController
        {
            private readonly ProgressDialog _dialog;
            private readonly ProgressDialogViewModel _viewModel;
            private bool _isClosed;

            public ProgressDialogController(ProgressDialog dialog, ProgressDialogViewModel viewModel)
            {
                _dialog = dialog;
                _viewModel = viewModel;
                _dialog.Closed += (_, _) => _isClosed = true;
            }

            public void UpdateMessage(string message)
            {
                InvokeOnUiThread(() =>
                {
                    if (_isClosed)
                        return;

                    _viewModel.Message = message;
                });
            }

            public void UpdateProgress(double value)
            {
                InvokeOnUiThread(() =>
                {
                    if (_isClosed)
                        return;

                    _viewModel.ProgressValue = value;
                });
            }

            public void SetIndeterminate(bool isIndeterminate)
            {
                InvokeOnUiThread(() =>
                {
                    if (_isClosed)
                        return;

                    _viewModel.IsIndeterminate = isIndeterminate;
                });
            }

            public void Close()
            {
                InvokeOnUiThread(() =>
                {
                    if (_isClosed)
                        return;

                    _isClosed = true;
                    _dialog.Close();
                });
            }

            public void Dispose() => Close();
        }
    }
}
