using System;

namespace LeakDetectSystem_MVVM.Models
{
    /// <summary>
    /// VisionPro 검사 결과를 담는 모델.
    /// CognexCameraService.RunInspection()의 반환값으로 사용됩니다.
    /// </summary>
    public class InspectionResult
    {
        /// <summary>검사 합격(true) / 불합격(false) 여부</summary>
        public bool Passed { get; set; }

        /// <summary>검사 수행 스테이션 인덱스 (1~4)</summary>
        public int StationIndex { get; set; }

        /// <summary>검사 완료 시각</summary>
        public DateTime InspectedAt { get; set; } = DateTime.Now;

        /// <summary>VPP Tool 실행 오류 메시지 (정상이면 null)</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>검사 성공 여부 (Passed 이고 오류 없음)</summary>
        public bool IsSuccess => Passed && string.IsNullOrEmpty(ErrorMessage);

        /// <summary>OK 결과 생성 편의 메서드</summary>
        public static InspectionResult Ok(int stationIndex) => new InspectionResult
        {
            Passed = true,
            StationIndex = stationIndex,
        };

        /// <summary>NG 결과 생성 편의 메서드</summary>
        public static InspectionResult Ng(int stationIndex, string? errorMessage = null) => new InspectionResult
        {
            Passed = false,
            StationIndex = stationIndex,
            ErrorMessage = errorMessage,
        };
    }
}
