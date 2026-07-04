using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LeakDetectSystem_MVVM.Models
{
    public class ModelConfig : INotifyPropertyChanged
    {
        private string _modelName = string.Empty;
        private string _cam1VppName = string.Empty;
        private string _cam2VppName = string.Empty;
        private string _cam3VppName = string.Empty;
        private string _cam4VppName = string.Empty;

        public string ModelName
        {
            get => _modelName;
            set { if (_modelName == value) return; _modelName = value; OnPropertyChanged(); }
        }

        public string Cam1VppName
        {
            get => _cam1VppName;
            set { if (_cam1VppName == value) return; _cam1VppName = value; OnPropertyChanged(); }
        }

        public string Cam2VppName
        {
            get => _cam2VppName;
            set { if (_cam2VppName == value) return; _cam2VppName = value; OnPropertyChanged(); }
        }

        public string Cam3VppName
        {
            get => _cam3VppName;
            set { if (_cam3VppName == value) return; _cam3VppName = value; OnPropertyChanged(); }
        }

        public string Cam4VppName
        {
            get => _cam4VppName;
            set { if (_cam4VppName == value) return; _cam4VppName = value; OnPropertyChanged(); }
        }

        public ModelConfig Clone() => new()
        {
            ModelName = ModelName,
            Cam1VppName = Cam1VppName,
            Cam2VppName = Cam2VppName,
            Cam3VppName = Cam3VppName,
            Cam4VppName = Cam4VppName
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
