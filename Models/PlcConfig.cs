namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// PLC 통신 설정 모델 – INI 파일에 저장/읽기되는 값을 담습니다.
    /// </summary>
    public class PlcConfig
    {
        public string IpAddress { get; set; } = "192.168.100.130";
        public string Port { get; set; } = "502";
        public string ReadAddress { get; set; } = "31000";
        public string ReadLength { get; set; } = "1000";
        public string ReadUnit { get; set; } = "0";
        public string WriteAddressOcr { get; set; } = "140";
        public string WriteAddress1 { get; set; } = "640";
        public string WriteAddress2 { get; set; } = "120";
        public string WriteUnit { get; set; } = "0";
        public string ReadInterval { get; set; } = "300";
        public string HeartbeatAddress { get; set; } = "621";
        public string DisplayMode { get; set; } = "Ascii";
    }
}
