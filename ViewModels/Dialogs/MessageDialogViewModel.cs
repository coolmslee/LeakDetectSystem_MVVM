using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Services;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class MessageDialogViewModel : DialogViewModelBase
    {
        private string _title;
        private string _header;
        private string _message;
        private string? _inputText;
        private bool _isInputVisible;
        private string _primaryButtonText = string.Empty;
        private string _secondaryButtonText = string.Empty;
        private bool _isPrimaryButtonVisible = true;
        private bool _isSecondaryButtonVisible;
        private bool _isPrimaryDefault = true;
        private bool _isSecondaryCancel;
        private MessageDialogResult _primaryResult = MessageDialogResult.OK;
        private MessageDialogResult _secondaryResult = MessageDialogResult.Cancel;

        public MessageDialogViewModel(MessageDialogRequest request)
        {
            _title = request.Title;
            _header = request.Header;
            _message = request.Message;
            _isInputVisible = request.IsInputVisible;
            _inputText = request.InputText;
            DialogType = request.DialogType;

            PrimaryCommand = new RelayCommand(HandlePrimaryAction);
            SecondaryCommand = new RelayCommand(HandleSecondaryAction);

            ConfigureButtons(request.Buttons);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string? InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        public bool IsInputVisible
        {
            get => _isInputVisible;
            set => SetProperty(ref _isInputVisible, value);
        }

        public MessageDialogType DialogType { get; }

        public string DialogTypeText => DialogType switch
        {
            MessageDialogType.Warning => "경고",
            MessageDialogType.Error => "오류",
            MessageDialogType.Question => "질문",
            _ => "안내"
        };

        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            private set => SetProperty(ref _primaryButtonText, value);
        }

        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            private set => SetProperty(ref _secondaryButtonText, value);
        }

        public bool IsPrimaryButtonVisible
        {
            get => _isPrimaryButtonVisible;
            private set => SetProperty(ref _isPrimaryButtonVisible, value);
        }

        public bool IsSecondaryButtonVisible
        {
            get => _isSecondaryButtonVisible;
            private set => SetProperty(ref _isSecondaryButtonVisible, value);
        }

        public bool IsPrimaryDefault
        {
            get => _isPrimaryDefault;
            private set => SetProperty(ref _isPrimaryDefault, value);
        }

        public bool IsSecondaryCancel
        {
            get => _isSecondaryCancel;
            private set => SetProperty(ref _isSecondaryCancel, value);
        }

        public MessageDialogResult Result { get; private set; } = MessageDialogResult.None;

        public RelayCommand PrimaryCommand { get; }
        public RelayCommand SecondaryCommand { get; }

        private void ConfigureButtons(MessageDialogButtons buttons)
        {
            IsPrimaryButtonVisible = true;
            IsSecondaryButtonVisible = false;
            IsPrimaryDefault = true;
            IsSecondaryCancel = false;
            _primaryResult = MessageDialogResult.OK;
            _secondaryResult = MessageDialogResult.Cancel;

            switch (buttons)
            {
                case MessageDialogButtons.OKCancel:
                    PrimaryButtonText = "확인";
                    SecondaryButtonText = "취소";
                    IsSecondaryButtonVisible = true;
                    IsSecondaryCancel = true;
                    _primaryResult = MessageDialogResult.OK;
                    _secondaryResult = MessageDialogResult.Cancel;
                    break;
                case MessageDialogButtons.ContinueStop:
                    PrimaryButtonText = "계속";
                    SecondaryButtonText = "중지";
                    IsSecondaryButtonVisible = true;
                    _primaryResult = MessageDialogResult.Continue;
                    _secondaryResult = MessageDialogResult.Stop;
                    break;
                case MessageDialogButtons.Close:
                    PrimaryButtonText = "닫기";
                    IsPrimaryDefault = false;
                    IsSecondaryCancel = true;
                    _primaryResult = MessageDialogResult.Close;
                    break;
                case MessageDialogButtons.OK:
                default:
                    PrimaryButtonText = "확인";
                    IsSecondaryCancel = true;
                    _primaryResult = MessageDialogResult.OK;
                    break;
            }
        }

        private void HandlePrimaryAction()
        {
            Result = _primaryResult;
            RequestClose();
        }

        private void HandleSecondaryAction()
        {
            Result = _secondaryResult;
            RequestClose();
        }
    }
}
