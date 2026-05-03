namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// 누설 감지 정보를 담는 데이터 모델.
    /// ViewModel과 View 사이에서 데이터를 전달하는 순수 데이터 클래스입니다.
    /// </summary>
    public class LeakInfoModel
    {
        /// <summary>스테이션(측정 지점) ID</summary>
        public int StationId { get; set; }

        /// <summary>스테이션 이름</summary>
        public string StationName { get; set; } = string.Empty;

        /// <summary>누설 감지 여부</summary>
        public bool IsLeakDetected { get; set; }

        /// <summary>측정된 압력 값 (단위: kPa)</summary>
        public double PressureValue { get; set; }

        /// <summary>누설 임계값 (단위: kPa)</summary>
        public double Threshold { get; set; }

        /// <summary>마지막 측정 시각</summary>
        public DateTime LastMeasuredAt { get; set; }

        /// <summary>상태 메시지</summary>
        public string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// 현재 압력이 임계값을 초과하는지 반환합니다.
        /// </summary>
        public bool IsAboveThreshold => PressureValue > Threshold;
    }
}
