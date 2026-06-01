using LeakDetectSystem_MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class CameraConfigIniService : ICameraConfigService
    {
        private readonly string _filePath;

        public CameraConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "camera.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public List<CameraConfig> Load()
        {
            var result = new List<CameraConfig>();

            for (int i = 1; i <= 4; i++)
            {
                string section = $"CAM{i}";

                var camera = new CameraConfig
                {
                    Index = i,
                    Use = ReadBool(section, "Use", false),
                    Ip = ReadString(section, "Ip", string.Empty),
                    VideoFormat = ReadString(section, "VideoFormat", "Mono8"),
                    ExposureTime = ReadInt(section, "ExposureTime", 10000),
                    Gain = ReadInt(section, "Gain", 100),
                    TimeoutEnabled = ReadBool(section, "TimeoutEnabled", true),
                    Timeout = ReadInt(section, "Timeout", 3000)
                };

                result.Add(camera);
            }

            return result;
        }

        public void Save(IEnumerable<CameraConfig> cameras)
        {
            foreach (var camera in cameras)
            {
                string section = $"CAM{camera.Index}";

                WriteString(section, "Use", camera.Use ? "1" : "0");
                WriteString(section, "Ip", camera.Ip);
                WriteString(section, "VideoFormat", camera.VideoFormat);
                WriteString(section, "ExposureTime", camera.ExposureTime.ToString());
                WriteString(section, "Gain", camera.Gain.ToString());
                WriteString(section, "TimeoutEnabled", camera.TimeoutEnabled ? "1" : "0");
                WriteString(section, "Timeout", camera.Timeout.ToString());
            }
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
