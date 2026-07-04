using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface IGrabConfigService
    {
        GrabConfig Load();
        void Save(GrabConfig config);
    }
}
