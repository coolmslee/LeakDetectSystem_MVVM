namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// Setting 탭 파라미터 설정 모델 – SetParameter.ini에 저장/읽기되는 값을 담습니다.
    /// </summary>
    public class SetParameterConfig
    {
        public string PlcStartAddress { get; set; } = "M2500";
        public string PcStartAddress { get; set; } = "M2550";
        public string PlcHeartBeatAddress { get; set; } = "0";
        public string PlcProductPresentAddress { get; set; } = "1";
        public string PlcBottleRollingAddress { get; set; } = "2";
        public string PlcInspectRequestAddress { get; set; } = "3";
        public string PlcReset1ReqAddress { get; set; } = "4";
        public string PlcReset2AckAddress { get; set; } = "5";
        public string PlcBottleDataAckAddress { get; set; } = "6";
        public string PcHeartBeatAddress { get; set; } = "0";
        public string PcVisionReadyAddress { get; set; } = "1";
        public string PcInspectDoneAddress { get; set; } = "2";
        public string PcReset1AckAddress { get; set; } = "3";
        public string PcReset2ReqAddress { get; set; } = "4";
        public string PcBottleDataReqAddress { get; set; } = "5";
        public int BottleTurnTime { get; set; } = 500;
        public int InspectReqTime { get; set; } = 300;
        public int InspectEndTime { get; set; } = 200;
    }
}
