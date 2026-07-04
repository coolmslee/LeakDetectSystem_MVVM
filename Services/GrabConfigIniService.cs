using LeakDetectSystem_MVVM.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class GrabConfigIniService : IGrabConfigService
    {
        private const string Section = "GRAB";
        private readonly string _filePath;

        public GrabConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Grab.ini");
            string? directoryPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public GrabConfig Load()
        {
            var config = new GrabConfig
            {
                Interval = ReadInt(Section, nameof(GrabConfig.Interval), 100),
                ImageSave = ReadBool(Section, nameof(GrabConfig.ImageSave), true),
                ImageExtension = ReadString(Section, nameof(GrabConfig.ImageExtension), "BMP"),
                HddSettingSpace = ReadInt(Section, nameof(GrabConfig.HddSettingSpace), 10)
            };

            config.ImageExtension = NormalizeImageExtension(config.ImageExtension);
            config.Interval = Math.Max(1, config.Interval);
            config.HddSettingSpace = Math.Max(1, config.HddSettingSpace);

            return config;
        }

        public void Save(GrabConfig config)
        {
            WriteString(Section, nameof(GrabConfig.Interval), Math.Max(1, config.Interval).ToString());
            WriteString(Section, nameof(GrabConfig.ImageSave), config.ImageSave ? "1" : "0");
            WriteString(Section, nameof(GrabConfig.ImageExtension), NormalizeImageExtension(config.ImageExtension));
            WriteString(Section, nameof(GrabConfig.HddSettingSpace), Math.Max(1, config.HddSettingSpace).ToString());
        }

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
            string value = ReadString(section, key, defaultValue ? "1" : "0");
            if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }

            return value == "1";
        }

        private void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }

        private static string NormalizeImageExtension(string? extension)
        {
            return string.Equals(extension?.Trim(), "JPEG", StringComparison.OrdinalIgnoreCase)
                ? "JPEG"
                : "BMP";
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
        private static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);
    }
}
