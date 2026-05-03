using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LeakDetectSystem_MVVM.ViewModels.Base
{
    /// <summary>
    /// 모든 ViewModel의 기반 클래스. INotifyPropertyChanged를 구현하고
    /// SetProperty 헬퍼를 제공하여 속성 변경 알림을 간결하게 처리합니다.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// PropertyChanged 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="propertyName">변경된 속성 이름 (자동 추론 가능)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 필드 값을 변경하고, 값이 실제로 바뀐 경우 PropertyChanged를 발생시킵니다.
        /// </summary>
        /// <typeparam name="T">속성 타입</typeparam>
        /// <param name="field">backing field 참조</param>
        /// <param name="value">새 값</param>
        /// <param name="propertyName">속성 이름 (자동 추론 가능)</param>
        /// <returns>값이 변경되었으면 true, 아니면 false</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 값 변경 후 추가 콜백을 실행하는 SetProperty 오버로드.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string? propertyName = null)
        {
            if (!SetProperty(ref field, value, propertyName))
                return false;

            onChanged?.Invoke();
            return true;
        }
    }
}
