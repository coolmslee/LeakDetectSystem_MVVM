using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LeakDetectSystem_MVVM.Models
{
    public class CameraConfig : INotifyPropertyChanged
    {
        // 유효 범위 상수
        public const int ExposureTimeMin = 100;
        public const int ExposureTimeMax = 1_000_000;
        public const int GainMin = 0;
        public const int GainMax = 1000;
        public const int TimeoutMin = 100;
        public const int TimeoutMax = 60_000;

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
                int clamped = Clamp(value, ExposureTimeMin, ExposureTimeMax);
                if (_exposureTime == clamped) return;
                _exposureTime = clamped;
                OnPropertyChanged();
            }
        }

        public int Gain
        {
            get => _gain;
            set
            {
                int clamped = Clamp(value, GainMin, GainMax);
                if (_gain == clamped) return;
                _gain = clamped;
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
                int clamped = Clamp(value, TimeoutMin, TimeoutMax);
                if (_timeout == clamped) return;
                _timeout = clamped;
                OnPropertyChanged();
            }
        }

        public bool IsConfigured => Use && !string.IsNullOrWhiteSpace(Ip);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static int Clamp(int value, int min, int max)
            => value < min ? min : value > max ? max : value;
    }
}
