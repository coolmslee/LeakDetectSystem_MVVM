using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class CameraDialogViewModel : ViewModelBase
    {
        private bool _cam1Use;
        private bool _cam2Use;
        private bool _cam3Use;
        private bool _cam4Use;
        private string _cam1Ip = string.Empty;
        private string _cam2Ip = string.Empty;
        private string _cam3Ip = string.Empty;
        private string _cam4Ip = string.Empty;
        private int _exposureTime = 10000;
        private int _gain = 100;

        public bool Cam1Use { get => _cam1Use; set => SetProperty(ref _cam1Use, value); }
        public bool Cam2Use { get => _cam2Use; set => SetProperty(ref _cam2Use, value); }
        public bool Cam3Use { get => _cam3Use; set => SetProperty(ref _cam3Use, value); }
        public bool Cam4Use { get => _cam4Use; set => SetProperty(ref _cam4Use, value); }

        public string Cam1Ip { get => _cam1Ip; set => SetProperty(ref _cam1Ip, value); }
        public string Cam2Ip { get => _cam2Ip; set => SetProperty(ref _cam2Ip, value); }
        public string Cam3Ip { get => _cam3Ip; set => SetProperty(ref _cam3Ip, value); }
        public string Cam4Ip { get => _cam4Ip; set => SetProperty(ref _cam4Ip, value); }

        public int ExposureTime
        {
            get => _exposureTime;
            set => SetProperty(ref _exposureTime, value);
        }

        public int Gain
        {
            get => _gain;
            set => SetProperty(ref _gain, value);
        }
    }
}
