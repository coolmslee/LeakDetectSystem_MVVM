using System;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class ProgressDialogViewModel : DialogViewModelBase
    {
        private string _title;
        private string _message;
        private double _progressValue;
        private bool _isIndeterminate;

        public ProgressDialogViewModel(
            string title = "진행 상태",
            string message = "작업을 준비하는 중입니다.",
            double progressValue = 0,
            bool isIndeterminate = true)
        {
            _title = title;
            _message = message;
            _progressValue = ClampProgress(progressValue);
            _isIndeterminate = isIndeterminate;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (SetProperty(ref _progressValue, ClampProgress(value)))
                    OnPropertyChanged(nameof(ProgressDisplay));
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                if (SetProperty(ref _isIndeterminate, value))
                    OnPropertyChanged(nameof(ProgressDisplay));
            }
        }

        public string ProgressDisplay => IsIndeterminate ? "진행 중..." : $"{ProgressValue:0}%";

        private static double ClampProgress(double value)
            => value < 0.0 ? 0.0 : value > 100.0 ? 100.0 : value;
    }
}
