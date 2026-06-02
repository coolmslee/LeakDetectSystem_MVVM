using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface IPlcConfigService
    {
        PlcConfig Load();
        void Save(PlcConfig config);
    }
}
