using LeakDetectSystem_MVVM.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class LightConfigIniService : ILightConfigService
    {
        private const string Section = "LIGHT";
        private readonly string _filePath;

        public LightConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "light.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public LightConfig Load()
        {
            return new LightConfig
            {
                Ch1Enabled  = ReadBool(Section, "Ch1Enabled",   true),
                Ch1Brightness = ReadInt(Section, "Ch1Brightness", 100),
                Ch2Enabled  = ReadBool(Section, "Ch2Enabled",   true),
                Ch2Brightness = ReadInt(Section, "Ch2Brightness", 100),
                Ch3Enabled  = ReadBool(Section, "Ch3Enabled",   false),
                Ch3Brightness = ReadInt(Section, "Ch3Brightness", 0),
                Ch4Enabled  = ReadBool(Section, "Ch4Enabled",   false),
                Ch4Brightness = ReadInt(Section, "Ch4Brightness", 0),
                ComPort       = ReadString(Section, "ComPort", string.Empty)
            };
        }

        public void Save(LightConfig config)
        {
            WriteString(Section, "Ch1Enabled",   config.Ch1Enabled   ? "1" : "0");
            WriteString(Section, "Ch1Brightness", config.Ch1Brightness.ToString());
            WriteString(Section, "Ch2Enabled",   config.Ch2Enabled   ? "1" : "0");
            WriteString(Section, "Ch2Brightness", config.Ch2Brightness.ToString());
            WriteString(Section, "Ch3Enabled",   config.Ch3Enabled   ? "1" : "0");
            WriteString(Section, "Ch3Brightness", config.Ch3Brightness.ToString());
            WriteString(Section, "Ch4Enabled",   config.Ch4Enabled   ? "1" : "0");
            WriteString(Section, "Ch4Brightness", config.Ch4Brightness.ToString());
            WriteString(Section, "ComPort",       config.ComPort);
        }

        private string ReadString(string section, string key, string defaultValue)
        {
            var sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _filePath);
            return sb.ToString();
        }

        private int ReadInt(string section, string key, int defaultValue)
        {
            return int.TryParse(ReadString(section, key, defaultValue.ToString()), out var value)
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
