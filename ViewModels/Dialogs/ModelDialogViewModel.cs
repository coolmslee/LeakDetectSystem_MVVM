using LeakDetectSystem_MVVM.ViewModels.Base;
using System.Collections.ObjectModel;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class ModelDialogViewModel : ViewModelBase
    {
        private string _modelName = string.Empty;
        private string _selectedModel = string.Empty;

        public string ModelName
        {
            get => _modelName;
            set => SetProperty(ref _modelName, value);
        }

        public string SelectedModel
        {
            get => _selectedModel;
            set => SetProperty(ref _selectedModel, value);
        }

        public ObservableCollection<string> Models { get; } = new() { "MODEL_A", "MODEL_B", "MODEL_C" };
    }
}
