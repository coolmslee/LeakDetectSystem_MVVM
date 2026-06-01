using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LeakDetectSystem_MVVM.Models
{
    public class CameraConfig : INotifyPropertyChanged
    {
        private bool _use;
        private string _ip = string.Empty;
        private string _videoFormat = "Mono8";
        private int _exposureTime = 10000;
        private int _gain = 100;
        private bool _timeoutEnabled = true;
        private int _timeout = 3000;

        public int Index { get; init; }
        public string Label => $"CAM{Index}";

        public bool Use
        {
            get => _use;
            set
            {
                if (_use == value) return;
                _use = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConfigured));
            }
        }

        public string Ip
        {
            get => _ip;
            set
            {
                if (_ip == value) return;
                _ip = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConfigured));
            }
        }

        public string VideoFormat
        {
            get => _videoFormat;
            set
            {
                if (_videoFormat == value) return;
                _videoFormat = value;
                OnPropertyChanged();
            }
        }

        public int ExposureTime
        {
            get => _exposureTime;
            set
            {
                if (_exposureTime == value) return;
                _exposureTime = value;
                OnPropertyChanged();
            }
        }

        public int Gain
        {
            get => _gain;
            set
            {
                if (_gain == value) return;
                _gain = value;
                OnPropertyChanged();
            }
        }

        public bool TimeoutEnabled
        {
            get => _timeoutEnabled;
            set
            {
                if (_timeoutEnabled == value) return;
                _timeoutEnabled = value;
                OnPropertyChanged();
            }
        }

        public int Timeout
        {
            get => _timeout;
            set
            {
                if (_timeout == value) return;
                _timeout = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfigured => Use && !string.IsNullOrWhiteSpace(Ip);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
