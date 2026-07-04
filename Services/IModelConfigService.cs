using LeakDetectSystem_MVVM.Models;
using System.Collections.Generic;

namespace LeakDetectSystem_MVVM.Services
{
    public interface IModelConfigService
    {
        List<ModelConfig> Load();
        void Save(IEnumerable<ModelConfig> models);
        string? LoadActiveModelName();
        void SaveActiveModelName(string modelName);
    }
}
