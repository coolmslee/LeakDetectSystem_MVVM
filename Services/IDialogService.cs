namespace LeakDetectSystem_MVVM.Services
{
    /// <summary>
    /// 다이얼로그(메시지박스, 파일 선택 등) 표시를 처리하는 서비스 인터페이스.
    /// ViewModel에서 UI 의존 없이 다이얼로그를 호출할 수 있도록 추상화합니다.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>정보 메시지를 표시합니다.</summary>
        void ShowMessage(string message, string title = "알림");

        /// <summary>확인/취소 다이얼로그를 표시하고 결과를 반환합니다.</summary>
        bool ShowConfirmation(string message, string title = "확인");

        /// <summary>오류 메시지를 표시합니다.</summary>
        void ShowError(string message, string title = "오류");

        /// <summary>파일 열기 다이얼로그를 표시하고 선택된 파일 경로를 반환합니다.</summary>
        string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*");

        /// <summary>파일 저장 다이얼로그를 표시하고 선택된 파일 경로를 반환합니다.</summary>
        string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*");
    }
}
