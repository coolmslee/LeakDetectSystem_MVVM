using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface ILightConfigService
    {
        LightConfig Load();
        void Save(LightConfig config);
    }
}
