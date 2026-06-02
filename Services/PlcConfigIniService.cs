using LeakDetectSystem_MVVM.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeakDetectSystem_MVVM.Services
{
    public class PlcConfigIniService : IPlcConfigService
    {
        private const string Section = "PLC";
        private readonly string _filePath;

        public PlcConfigIniService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "plc.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public PlcConfig Load()
        {
            return new PlcConfig
            {
                IpAddress       = ReadString(Section, "IpAddress",       "192.168.100.130"),
                Port            = ReadString(Section, "Port",            "502"),
                ReadAddress     = ReadString(Section, "ReadAddress",     "31000"),
                ReadLength      = ReadString(Section, "ReadLength",      "1000"),
                ReadUnit        = ReadString(Section, "ReadUnit",        "0"),
                WriteAddressOcr = ReadString(Section, "WriteAddressOcr", "140"),
                WriteAddress1   = ReadString(Section, "WriteAddress1",   "640"),
                WriteAddress2   = ReadString(Section, "WriteAddress2",   "120"),
                WriteUnit       = ReadString(Section, "WriteUnit",       "0"),
                ReadInterval    = ReadString(Section, "ReadInterval",    "300"),
                HeartbeatAddress = ReadString(Section, "HeartbeatAddress", "621"),
                DisplayMode     = ReadString(Section, "DisplayMode",     "Ascii")
            };
        }

        public void Save(PlcConfig config)
        {
            WriteString(Section, "IpAddress",       config.IpAddress);
            WriteString(Section, "Port",            config.Port);
            WriteString(Section, "ReadAddress",     config.ReadAddress);
            WriteString(Section, "ReadLength",      config.ReadLength);
            WriteString(Section, "ReadUnit",        config.ReadUnit);
            WriteString(Section, "WriteAddressOcr", config.WriteAddressOcr);
            WriteString(Section, "WriteAddress1",   config.WriteAddress1);
            WriteString(Section, "WriteAddress2",   config.WriteAddress2);
            WriteString(Section, "WriteUnit",       config.WriteUnit);
            WriteString(Section, "ReadInterval",    config.ReadInterval);
            WriteString(Section, "HeartbeatAddress", config.HeartbeatAddress);
            WriteString(Section, "DisplayMode",     config.DisplayMode);
        }

        private string ReadString(string section, string key, string defaultValue)
        {
            var sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, _filePath);
            return sb.ToString();
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
