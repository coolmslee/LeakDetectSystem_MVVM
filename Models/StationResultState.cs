namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// 스테이션 검사 결과 상태를 나타내는 열거형.
    /// 원본(LeakDetectSystem)의 OK/NG 판정 결과에 대응합니다.
    /// </summary>
    public enum StationResultState
    {
        /// <summary>측정 전 또는 결과 없음</summary>
        Unknown = 0,
        /// <summary>검사 합격 (누설 없음)</summary>
        OK = 1,
        /// <summary>검사 불합격 (누설 감지)</summary>
        NG = 2,
    }
}
