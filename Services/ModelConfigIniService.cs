using LeakDetectSystem_MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class ModelConfigIniService : IModelConfigService
    {
        private const string ModelsSection = "MODELS";
        private const string CountKey = "Count";
        private const string ModelKeyPrefix = "Model";
        private const int MaxIniValueLength = 512;
        private readonly string _filePath;

        public ModelConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Model.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public List<ModelConfig> Load()
        {
            var result = new List<ModelConfig>();
            int count = ReadInt(ModelsSection, CountKey, 0);

            for (int i = 1; i <= count; i++)
            {
                string sectionKey = ReadString(ModelsSection, $"{ModelKeyPrefix}{i}", string.Empty);
                if (string.IsNullOrWhiteSpace(sectionKey))
                    continue;

                result.Add(new ModelConfig
                {
                    ModelName = ReadString(sectionKey, "ModelName", sectionKey),
                    Cam1VppName = ReadString(sectionKey, "Cam1VppName", string.Empty),
                    Cam2VppName = ReadString(sectionKey, "Cam2VppName", string.Empty),
                    Cam3VppName = ReadString(sectionKey, "Cam3VppName", string.Empty),
                    Cam4VppName = ReadString(sectionKey, "Cam4VppName", string.Empty)
                });
            }

            return result;
        }

        public void Save(IEnumerable<ModelConfig> models)
        {
            var modelList = new List<ModelConfig>(models);
            WriteString(ModelsSection, CountKey, modelList.Count.ToString());

            for (int i = 0; i < modelList.Count; i++)
            {
                var model = modelList[i];
                string sectionKey = SanitizeSectionName(model.ModelName, i + 1);
                WriteString(ModelsSection, $"{ModelKeyPrefix}{i + 1}", sectionKey);
                WriteString(sectionKey, "ModelName", model.ModelName);
                WriteString(sectionKey, "Cam1VppName", model.Cam1VppName);
                WriteString(sectionKey, "Cam2VppName", model.Cam2VppName);
                WriteString(sectionKey, "Cam3VppName", model.Cam3VppName);
                WriteString(sectionKey, "Cam4VppName", model.Cam4VppName);
            }
        }

        private static string SanitizeSectionName(string name, int fallbackIndex)
        {
            var sb = new StringBuilder();
            foreach (char c in name)
            {
                if (c != '[' && c != ']' && c != '\n' && c != '\r')
                    sb.Append(c);
            }
            string sanitized = sb.ToString().Trim();
            return sanitized.Length > 0 ? sanitized : $"Model{fallbackIndex}";
        }

        private string ReadString(string section, string key, string defaultValue)
        {
            var sb = new StringBuilder(MaxIniValueLength);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _filePath);
            return sb.ToString();
        }

        private int ReadInt(string section, string key, int defaultValue)
        {
            return int.TryParse(ReadString(section, key, defaultValue.ToString()), out var value)
                ? value
                : defaultValue;
        }

        private void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpString, string lpFileName);
    }
}
