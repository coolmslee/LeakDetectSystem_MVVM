using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class PlcDialogViewModel : ViewModelBase
    {
        private string _ipAddress = "192.168.0.1";
        private int _port = 502;
        private int _slaveId = 1;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public int SlaveId
        {
            get => _slaveId;
            set => SetProperty(ref _slaveId, value);
        }
    }
}
