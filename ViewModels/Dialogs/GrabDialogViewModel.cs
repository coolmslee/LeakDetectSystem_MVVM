namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class GrabDialogViewModel : DialogViewModelBase
    {
        private int _triggerMode = 0;
        private int _grabTimeout = 5000;
        private bool _isContinuous;

        public int TriggerMode
        {
            get => _triggerMode;
            set => SetProperty(ref _triggerMode, value);
        }

        public int GrabTimeout
        {
            get => _grabTimeout;
            set => SetProperty(ref _grabTimeout, value);
        }

        public bool IsContinuous
        {
            get => _isContinuous;
            set => SetProperty(ref _isContinuous, value);
        }
    }
}
