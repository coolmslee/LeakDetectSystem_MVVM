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
        private string? _activeModelName;

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
            ApplyModelCommand = new RelayCommand(ApplyModel, () => HasSelectedModel);

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
                    ApplyModelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasSelectedModel => _selectedModel != null;

        public string? ActiveModelName
        {
            get => _activeModelName;
            private set => SetProperty(ref _activeModelName, value);
        }

        public RelayCommand AddModelCommand { get; }
        public RelayCommand EditModelCommand { get; }
        public RelayCommand DeleteModelCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ApplyModelCommand { get; }

        private void LoadModels()
        {
            Models.Clear();
            foreach (var model in _modelConfigService.Load())
                Models.Add(model);

            ActiveModelName = _modelConfigService.LoadActiveModelName();
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
            LoadModels();
            _dialogService.ShowMessage("모델 설정이 저장되었습니다.", "모델 설정");
        }

        private void ApplyModel()
        {
            if (_selectedModel == null) return;

            _modelConfigService.SaveActiveModelName(_selectedModel.ModelName);
            ActiveModelName = _selectedModel.ModelName;
            _dialogService.ShowMessage($"'{_selectedModel.ModelName}' 모델이 적용되었습니다.", "모델 적용");
        }
    }
}

