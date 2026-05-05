using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class LightDialogViewModel : ViewModelBase
    {
        private int _channel1 = 100;
        private int _channel2 = 100;
        private int _channel3 = 0;
        private int _channel4 = 0;

        public int Channel1 { get => _channel1; set => SetProperty(ref _channel1, value); }
        public int Channel2 { get => _channel2; set => SetProperty(ref _channel2, value); }
        public int Channel3 { get => _channel3; set => SetProperty(ref _channel3, value); }
        public int Channel4 { get => _channel4; set => SetProperty(ref _channel4, value); }
    }
}
