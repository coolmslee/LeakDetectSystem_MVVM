namespace LeakDetectSystem_MVVM.Services
{
    public enum MessageDialogButtons
    {
        OK,
        OKCancel,
        ContinueStop,
        Close
    }

    public enum MessageDialogType
    {
        Info,
        Warning,
        Error,
        Question
    }

    public enum MessageDialogResult
    {
        None,
        OK,
        Cancel,
        Continue,
        Stop,
        Close
    }

    public sealed class MessageDialogRequest
    {
        public string Title { get; set; } = "알림";
        public string Header { get; set; } = "알림";
        public string Message { get; set; } = string.Empty;
        public bool IsInputVisible { get; set; }
        public string? InputText { get; set; }
        public MessageDialogButtons Buttons { get; set; } = MessageDialogButtons.OK;
        public MessageDialogType DialogType { get; set; } = MessageDialogType.Info;
    }
}
