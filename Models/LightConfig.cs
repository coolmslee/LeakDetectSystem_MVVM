namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// 조명 설정 모델 – INI 파일에 저장/읽기되는 값을 담습니다.
    /// </summary>
    public class LightConfig
    {
        public bool Ch1Enabled { get; set; } = true;
        public int Ch1Brightness { get; set; } = 100;

        public bool Ch2Enabled { get; set; } = true;
        public int Ch2Brightness { get; set; } = 100;

        public bool Ch3Enabled { get; set; } = false;
        public int Ch3Brightness { get; set; } = 0;

        public bool Ch4Enabled { get; set; } = false;
        public int Ch4Brightness { get; set; } = 0;

        public string ComPort { get; set; } = string.Empty;
    }
}
