using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Models;
using LeakDetectSystem_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public class ModelDialogViewModel : DialogViewModelBase
    {
        private readonly IModelConfigService _modelConfigService;
        private readonly IDialogService _dialogService;
        private ModelConfig? _selectedModel;

        public ModelDialogViewModel()
            : this(new ModelConfigIniService(), new CameraConfigIniService(), new DialogService())
        {
        }

        public ModelDialogViewModel(
            IModelConfigService modelConfigService,
            ICameraConfigService cameraConfigService,
            IDialogService dialogService)
        {
            _modelConfigService = modelConfigService;
            _dialogService = dialogService;

            Models = new ObservableCollection<ModelConfig>();

            var cameras = cameraConfigService.Load();
            CameraConfigs = Enumerable.Range(1, 4)
                .Select(i => cameras.FirstOrDefault(c => c.Index == i) ?? new CameraConfig { Index = i })
                .ToList();

            AddModelCommand = new RelayCommand(AddModel);
            EditModelCommand = new RelayCommand(EditModel, () => HasSelectedModel);
            DeleteModelCommand = new RelayCommand(DeleteModel, () => HasSelectedModel);
            SaveCommand = new RelayCommand(Save);
            BrowseModelCommand = new RelayCommand(BrowseModel, () => HasSelectedModel);

            LoadModels();
        }

        public ObservableCollection<ModelConfig> Models { get; }

        /// <summary>카메라 설정값 (camera.ini 에서 로드, 표시 전용).</summary>
        public List<CameraConfig> CameraConfigs { get; }

        public ModelConfig? SelectedModel
        {
            get => _selectedModel;
            set
            {
                if (SetProperty(ref _selectedModel, value))
                {
                    OnPropertyChanged(nameof(HasSelectedModel));
                    EditModelCommand.RaiseCanExecuteChanged();
                    DeleteModelCommand.RaiseCanExecuteChanged();
                    BrowseModelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasSelectedModel => _selectedModel != null;

        public RelayCommand AddModelCommand { get; }
        public RelayCommand EditModelCommand { get; }
        public RelayCommand DeleteModelCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand BrowseModelCommand { get; }

        private void LoadModels()
        {
            Models.Clear();
            foreach (var model in _modelConfigService.Load())
                Models.Add(model);

            SelectedModel = Models.FirstOrDefault();
        }

        private void AddModel()
        {
            var newModel = new ModelConfig { ModelName = $"Model_{Models.Count + 1}" };
            Models.Add(newModel);
            SelectedModel = newModel;
        }

        private void EditModel()
        {
            if (_selectedModel == null) return;

            string? newName = _dialogService.ShowInputDialog(
                "새 모델명을 입력하세요.",
                "모델 이름 변경",
                _selectedModel.ModelName);

            if (!string.IsNullOrWhiteSpace(newName))
                _selectedModel.ModelName = newName;
        }

        private void DeleteModel()
        {
            if (_selectedModel == null) return;

            bool confirmed = _dialogService.ShowConfirmation(
                $"'{_selectedModel.ModelName}' 모델을 삭제하시겠습니까?", "모델 삭제");

            if (!confirmed) return;

            int index = Models.IndexOf(_selectedModel);
            Models.Remove(_selectedModel);
            SelectedModel = Models.Count > 0
                ? Models[Math.Min(index, Models.Count - 1)]
                : null;
        }

        private void Save()
        {
            _modelConfigService.Save(Models);
            _dialogService.ShowMessage("모델 설정이 저장되고 즉시 적용되었습니다.", "모델 설정");
        }

        private void BrowseModel()
        {
            if (_selectedModel == null) return;

            string? filePath = _dialogService.ShowOpenFileDialog("VPP 파일 (*.vpp)|*.vpp|모든 파일 (*.*)|*.*");
            if (filePath != null)
                _selectedModel.ModelName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }
    }
}

