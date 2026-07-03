using LeakDetectSystem_MVVM.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class SetParameterIniService : ISetParameterConfigService
    {
        private const string Section = "SETTING";
        private readonly string _filePath;

        public SetParameterIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "SetParameter.ini");
            string? directoryPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public SetParameterConfig Load()
        {
            return new SetParameterConfig
            {
                PlcStartAddress = ReadString(Section, nameof(SetParameterConfig.PlcStartAddress), "M2500"),
                PcStartAddress = ReadString(Section, nameof(SetParameterConfig.PcStartAddress), "M2550"),
                PlcHeartBeatAddress = ReadString(Section, nameof(SetParameterConfig.PlcHeartBeatAddress), "0"),
                PlcProductPresentAddress = ReadString(Section, nameof(SetParameterConfig.PlcProductPresentAddress), "1"),
                PlcBottleRollingAddress = ReadString(Section, nameof(SetParameterConfig.PlcBottleRollingAddress), "2"),
                PlcInspectRequestAddress = ReadString(Section, nameof(SetParameterConfig.PlcInspectRequestAddress), "3"),
                PlcReset1ReqAddress = ReadString(Section, nameof(SetParameterConfig.PlcReset1ReqAddress), "4"),
                PlcReset2AckAddress = ReadString(Section, nameof(SetParameterConfig.PlcReset2AckAddress), "5"),
                PlcBottleDataAckAddress = ReadString(Section, nameof(SetParameterConfig.PlcBottleDataAckAddress), "6"),
                PcHeartBeatAddress = ReadString(Section, nameof(SetParameterConfig.PcHeartBeatAddress), "0"),
                PcVisionReadyAddress = ReadString(Section, nameof(SetParameterConfig.PcVisionReadyAddress), "1"),
                PcInspectDoneAddress = ReadString(Section, nameof(SetParameterConfig.PcInspectDoneAddress), "2"),
                PcReset1AckAddress = ReadString(Section, nameof(SetParameterConfig.PcReset1AckAddress), "3"),
                PcReset2ReqAddress = ReadString(Section, nameof(SetParameterConfig.PcReset2ReqAddress), "4"),
                PcBottleDataReqAddress = ReadString(Section, nameof(SetParameterConfig.PcBottleDataReqAddress), "5"),
                BottleTurnTime = ReadInt(Section, nameof(SetParameterConfig.BottleTurnTime), 500),
                InspectReqTime = ReadInt(Section, nameof(SetParameterConfig.InspectReqTime), 300),
                InspectEndTime = ReadInt(Section, nameof(SetParameterConfig.InspectEndTime), 200)
            };
        }

        public void Save(SetParameterConfig config)
        {
            WriteString(Section, nameof(SetParameterConfig.PlcStartAddress), config.PlcStartAddress);
            WriteString(Section, nameof(SetParameterConfig.PcStartAddress), config.PcStartAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcHeartBeatAddress), config.PlcHeartBeatAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcProductPresentAddress), config.PlcProductPresentAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcBottleRollingAddress), config.PlcBottleRollingAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcInspectRequestAddress), config.PlcInspectRequestAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcReset1ReqAddress), config.PlcReset1ReqAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcReset2AckAddress), config.PlcReset2AckAddress);
            WriteString(Section, nameof(SetParameterConfig.PlcBottleDataAckAddress), config.PlcBottleDataAckAddress);
            WriteString(Section, nameof(SetParameterConfig.PcHeartBeatAddress), config.PcHeartBeatAddress);
            WriteString(Section, nameof(SetParameterConfig.PcVisionReadyAddress), config.PcVisionReadyAddress);
            WriteString(Section, nameof(SetParameterConfig.PcInspectDoneAddress), config.PcInspectDoneAddress);
            WriteString(Section, nameof(SetParameterConfig.PcReset1AckAddress), config.PcReset1AckAddress);
            WriteString(Section, nameof(SetParameterConfig.PcReset2ReqAddress), config.PcReset2ReqAddress);
            WriteString(Section, nameof(SetParameterConfig.PcBottleDataReqAddress), config.PcBottleDataReqAddress);
            WriteString(Section, nameof(SetParameterConfig.BottleTurnTime), config.BottleTurnTime.ToString());
            WriteString(Section, nameof(SetParameterConfig.InspectReqTime), config.InspectReqTime.ToString());
            WriteString(Section, nameof(SetParameterConfig.InspectEndTime), config.InspectEndTime.ToString());
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
        private static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);
    }
}
