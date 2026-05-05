using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LeakDetectSystem_MVVM.Models
{
    public class CameraConfig : INotifyPropertyChanged
    {
        private bool _use;
        private string _ip = string.Empty;

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

        public bool IsConfigured => Use && !string.IsNullOrWhiteSpace(Ip);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
