using System.Collections.Generic;

namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// 조명 설정 모델 – 여러 컨트롤러/포트와 채널 구성을 INI에 저장/읽기합니다.
    /// </summary>
    public class LightConfig
    {
        public List<LightControllerConfig> Controllers { get; set; } = LightControllerConfig.CreateDefaults();

        public static LightConfig CreateDefault(int controllerCount = 2)
        {
            return new LightConfig
            {
                Controllers = LightControllerConfig.CreateDefaults(controllerCount)
            };
        }
    }

    public class LightControllerConfig
    {
        public string Name { get; set; } = string.Empty;
        public string ComPort { get; set; } = string.Empty;
        public bool Use { get; set; } = true;
        public List<LightChannelConfig> Channels { get; set; } = LightChannelConfig.CreateDefaults();

        public static LightControllerConfig CreateDefault(int index)
        {
            return new LightControllerConfig
            {
                Name = $"Light Controller {index}",
                ComPort = $"COM{index}",
                Use = true,
                Channels = LightChannelConfig.CreateDefaults()
            };
        }

        public static List<LightControllerConfig> CreateDefaults(int controllerCount = 2)
        {
            controllerCount = controllerCount < 1 ? 1 : controllerCount;

            var controllers = new List<LightControllerConfig>(controllerCount);
            for (int i = 1; i <= controllerCount; i++)
            {
                controllers.Add(CreateDefault(i));
            }

            return controllers;
        }
    }

    public class LightChannelConfig
    {
        public string Name { get; set; } = string.Empty;
        public bool Use { get; set; }
        public int Brightness { get; set; }

        public static LightChannelConfig CreateDefault(int index)
        {
            bool enabledByDefault = index <= 2;

            return new LightChannelConfig
            {
                Name = $"CH{index}",
                Use = enabledByDefault,
                Brightness = enabledByDefault ? 100 : 0
            };
        }

        public static List<LightChannelConfig> CreateDefaults(int channelCount = 4)
        {
            channelCount = channelCount < 4 ? 4 : channelCount;

            var channels = new List<LightChannelConfig>(channelCount);
            for (int i = 1; i <= channelCount; i++)
            {
                channels.Add(CreateDefault(i));
            }

            return channels;
        }
    }
}
