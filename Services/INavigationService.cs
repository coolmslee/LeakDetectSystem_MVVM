namespace LeakDetectSystem_MVVM.Services
{
    /// <summary>
    /// 화면/페이지 간 이동을 처리하는 네비게이션 서비스 인터페이스.
    /// 실제 구현체는 NavigationService 클래스에서 제공합니다.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>현재 표시 중인 뷰 이름</summary>
        string? CurrentView { get; }

        /// <summary>지정된 뷰 이름으로 이동합니다.</summary>
        void NavigateTo(string viewName, object? parameter = null);

        /// <summary>이전 뷰로 돌아갑니다.</summary>
        void GoBack();

        /// <summary>뒤로 이동 가능 여부</summary>
        bool CanGoBack { get; }
    }
}
