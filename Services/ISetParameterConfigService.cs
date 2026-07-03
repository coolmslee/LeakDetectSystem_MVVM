using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface ISetParameterConfigService
    {
        SetParameterConfig Load();
        void Save(SetParameterConfig config);
    }
}
