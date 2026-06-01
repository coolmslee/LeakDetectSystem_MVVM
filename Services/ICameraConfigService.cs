using System.Collections.Generic;
using LeakDetectSystem_MVVM.Models;

namespace LeakDetectSystem_MVVM.Services
{
    public interface ICameraConfigService
    {
        List<CameraConfig> Load();
        void Save(IEnumerable<CameraConfig> cameras);
    }
}
