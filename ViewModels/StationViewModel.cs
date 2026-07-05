using System;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.ViewModels.Base;

namespace LeakDetectSystem_MVVM.ViewModels
{
    /// <summary>
    /// 개별 스테이션(측정 지점) 패널에 대한 ViewModel.
    /// StationView.xaml에 DataContext로 바인딩됩니다.
    /// </summary>
    public class StationViewModel : ViewModelBase
    {
        private int _stationId;
        private string _stationName = string.Empty;
        private bool _isLeakDetected;
        private double _pressureValue;
        private double _threshold;
        private DateTime _lastMeasuredAt;
        private bool _isMonitoring;

        public int StationId
        {
            get => _stationId;
            set => SetProperty(ref _stationId, value);
        }

        public string StationName
        {
            get => _stationName;
            set => SetProperty(ref _stationName, value);
        }

        public bool IsLeakDetected
        {
            get => _isLeakDetected;
            set => SetProperty(ref _isLeakDetected, value, () => OnPropertyChanged(nameof(StatusMessage)));
        }

        public double PressureValue
        {
            get => _pressureValue;
            set => SetProperty(ref _pressureValue, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        public double Threshold
        {
            get => _threshold;
            set => SetProperty(ref _threshold, value, () => OnPropertyChanged(nameof(IsAboveThreshold)));
        }

        public DateTime LastMeasuredAt
        {
            get => _lastMeasuredAt;
            set => SetProperty(ref _lastMeasuredAt, value);
        }

        public string StatusMessage => IsLeakDetected ? "⚠ 누설 감지됨" : "✔ 정상";

        public bool IsAboveThreshold => PressureValue > Threshold;

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        /// <summary>
        /// LeakInfoModel로부터 ViewModel을 초기화합니다.
        /// </summary>
        public static StationViewModel FromModel(LeakInfoModel model) => new()
        {
            StationId = model.StationId,
            StationName = model.StationName,
            IsLeakDetected = model.IsLeakDetected,
            PressureValue = model.PressureValue,
            Threshold = model.Threshold,
            LastMeasuredAt = model.LastMeasuredAt,
        };
    }
}
