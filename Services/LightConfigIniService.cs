using LeakDetectSystem_MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class LightConfigIniService : ILightConfigService
    {
        private const string RootSection = "LIGHT";
        private const int DefaultControllerCount = 2;
        private const int MinimumChannelCount = 4;
        private readonly string _filePath;

        public LightConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "light.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public LightConfig Load()
        {
            if (!File.Exists(_filePath))
            {
                return LightConfig.CreateDefault(DefaultControllerCount);
            }

            int controllerCount = ReadInt(RootSection, "ControllerCount", 0);
            if (controllerCount > 0)
            {
                return LoadControllerConfig(controllerCount);
            }

            return LoadLegacyConfig();
        }

        public void Save(LightConfig config)
        {
            var normalized = NormalizeConfig(config);
            string tempPath = $"{_filePath}.tmp";

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            WriteString(tempPath, RootSection, "ControllerCount", normalized.Controllers.Count.ToString());

            for (int controllerIndex = 0; controllerIndex < normalized.Controllers.Count; controllerIndex++)
            {
                LightControllerConfig controller = normalized.Controllers[controllerIndex];
                string controllerSection = GetControllerSection(controllerIndex + 1);

                WriteString(tempPath, controllerSection, "Name", controller.Name);
                WriteString(tempPath, controllerSection, "ComPort", controller.ComPort);
                WriteString(tempPath, controllerSection, "Use", controller.Use ? "1" : "0");
                WriteString(tempPath, controllerSection, "ChannelCount", controller.Channels.Count.ToString());

                for (int channelIndex = 0; channelIndex < controller.Channels.Count; channelIndex++)
                {
                    LightChannelConfig channel = controller.Channels[channelIndex];
                    string channelSection = GetChannelSection(controllerIndex + 1, channelIndex + 1);

                    WriteString(tempPath, channelSection, "Name", channel.Name);
                    WriteString(tempPath, channelSection, "Use", channel.Use ? "1" : "0");
                    WriteString(tempPath, channelSection, "Brightness", NormalizeBrightness(channel.Brightness).ToString());
                }
            }

            if (File.Exists(_filePath))
            {
                File.Replace(tempPath, _filePath, null);
            }
            else
            {
                File.Move(tempPath, _filePath);
            }
        }

        private LightConfig LoadControllerConfig(int controllerCount)
        {
            var controllers = new List<LightControllerConfig>();

            for (int controllerIndex = 1; controllerIndex <= controllerCount; controllerIndex++)
            {
                string controllerSection = GetControllerSection(controllerIndex);
                int channelCount = ReadInt(controllerSection, "ChannelCount", MinimumChannelCount);
                if (channelCount < MinimumChannelCount)
                {
                    channelCount = MinimumChannelCount;
                }

                var controller = new LightControllerConfig
                {
                    Name = ReadString(controllerSection, "Name", $"Light Controller {controllerIndex}"),
                    ComPort = ReadString(controllerSection, "ComPort", $"COM{controllerIndex}"),
                    Use = ReadBool(controllerSection, "Use", true),
                    Channels = new List<LightChannelConfig>(channelCount)
                };

                for (int channelIndex = 1; channelIndex <= channelCount; channelIndex++)
                {
                    string channelSection = GetChannelSection(controllerIndex, channelIndex);
                    bool defaultUse = channelIndex <= 2;

                    controller.Channels.Add(new LightChannelConfig
                    {
                        Name = ReadString(channelSection, "Name", $"CH{channelIndex}"),
                        Use = ReadBool(channelSection, "Use", defaultUse),
                        Brightness = NormalizeBrightness(ReadInt(channelSection, "Brightness", defaultUse ? 100 : 0))
                    });
                }

                controllers.Add(controller);
            }

            return NormalizeConfig(new LightConfig { Controllers = controllers });
        }

        private LightConfig LoadLegacyConfig()
        {
            var config = LightConfig.CreateDefault(1);
            LightControllerConfig controller = config.Controllers[0];

            controller.ComPort = ReadString(RootSection, "ComPort", controller.ComPort);
            controller.Use = true;

            ApplyLegacyChannel(controller.Channels[0], "Ch1", true, 100);
            ApplyLegacyChannel(controller.Channels[1], "Ch2", true, 100);
            ApplyLegacyChannel(controller.Channels[2], "Ch3", false, 0);
            ApplyLegacyChannel(controller.Channels[3], "Ch4", false, 0);

            return NormalizeConfig(config);
        }

        private void ApplyLegacyChannel(LightChannelConfig channel, string keyPrefix, bool defaultUse, int defaultBrightness)
        {
            channel.Use = ReadBool(RootSection, $"{keyPrefix}Enabled", defaultUse);
            channel.Brightness = NormalizeBrightness(ReadInt(RootSection, $"{keyPrefix}Brightness", defaultBrightness));
        }

        private static LightConfig NormalizeConfig(LightConfig? config)
        {
            var controllers = config?.Controllers ?? new List<LightControllerConfig>();
            if (controllers.Count == 0)
            {
                return LightConfig.CreateDefault(DefaultControllerCount);
            }

            var normalizedControllers = new List<LightControllerConfig>(controllers.Count);

            for (int controllerIndex = 0; controllerIndex < controllers.Count; controllerIndex++)
            {
                LightControllerConfig source = controllers[controllerIndex] ?? LightControllerConfig.CreateDefault(controllerIndex + 1);
                var normalizedController = new LightControllerConfig
                {
                    Name = string.IsNullOrWhiteSpace(source.Name) ? $"Light Controller {controllerIndex + 1}" : source.Name.Trim(),
                    ComPort = source.ComPort?.Trim() ?? string.Empty,
                    Use = source.Use,
                    Channels = new List<LightChannelConfig>()
                };

                List<LightChannelConfig> sourceChannels = source.Channels ?? new List<LightChannelConfig>();
                int channelCount = Math.Max(MinimumChannelCount, sourceChannels.Count);

                for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
                {
                    LightChannelConfig channel = channelIndex < sourceChannels.Count
                        ? sourceChannels[channelIndex]
                        : LightChannelConfig.CreateDefault(channelIndex + 1);

                    normalizedController.Channels.Add(new LightChannelConfig
                    {
                        Name = string.IsNullOrWhiteSpace(channel.Name) ? $"CH{channelIndex + 1}" : channel.Name.Trim(),
                        Use = channel.Use,
                        Brightness = NormalizeBrightness(channel.Brightness)
                    });
                }

                normalizedControllers.Add(normalizedController);
            }

            return new LightConfig { Controllers = normalizedControllers };
        }

        private static int NormalizeBrightness(int value) => Math.Clamp(value, 0, 480);

        private static string GetControllerSection(int controllerIndex) => $"LIGHT_CONTROLLER_{controllerIndex}";
        private static string GetChannelSection(int controllerIndex, int channelIndex) => $"LIGHT_CONTROLLER_{controllerIndex}_CHANNEL_{channelIndex}";

        private string ReadString(string section, string key, string defaultValue)
        {
            var sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _filePath);
            return sb.ToString();
        }

        private int ReadInt(string section, string key, int defaultValue)
        {
            return int.TryParse(ReadString(section, key, defaultValue.ToString()), out int value)
                ? value
                : defaultValue;
        }

        private bool ReadBool(string section, string key, bool defaultValue)
        {
            return ReadString(section, key, defaultValue ? "1" : "0") == "1";
        }

        private void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }

        private static void WriteString(string filePath, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);
    }
}
