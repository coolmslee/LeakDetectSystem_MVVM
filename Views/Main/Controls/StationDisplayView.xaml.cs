using System;
using System.Windows;
using System.Windows.Controls;
using Cognex.VisionPro.Display;
using LeakDetectSystem_MVVM.ViewModels;

namespace LeakDetectSystem_MVVM.Views.Main.Controls
{
    /// <summary>
    /// StationDisplayView 코드비하인드.
    /// CogDisplay(WinForms) 를 WindowsFormsHost 에 연결하고,
    /// ViewModel 이벤트 구독을 통해 획득 이미지를 표시합니다.
    /// </summary>
    public partial class StationDisplayView : UserControl
    {
        private CogDisplay? _cogDisplay;
        private StationCardViewModel? _vm;

        public StationDisplayView()
        {
            InitializeComponent();

            // CogDisplay 생성 및 WindowsFormsHost 에 연결
            _cogDisplay = new CogDisplay
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
            };
            CogDisplayHost.Child = _cogDisplay;

            // DataContext 가 바뀔 때마다 VM 구독을 갱신합니다.
            DataContextChanged += OnDataContextChanged;

            // Unloaded 시 리소스 정리
            Unloaded += OnUnloaded;
        }

        // ── DataContext 구독 관리 ──────────────────────────────────────────

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // 이전 VM 구독 해제
            if (e.OldValue is StationCardViewModel oldVm)
                oldVm.ImageAcquired -= OnImageAcquired;

            // 새 VM 구독 등록 및 CogDisplay 전달
            _vm = e.NewValue as StationCardViewModel;
            if (_vm != null)
            {
                _vm.ImageAcquired += OnImageAcquired;

                // CogDisplay 인스턴스를 ViewModel 을 통해 서비스에 전달
                if (_cogDisplay != null)
                    _vm.AttachDisplay(_cogDisplay);
            }
        }

        // ── 이미지 수신 ───────────────────────────────────────────────────

        private void OnImageAcquired(Cognex.VisionPro.ICogImage image)
        {
            // UI 스레드에서 CogDisplay 갱신
            Dispatcher.Invoke(() =>
            {
                if (_cogDisplay == null) return;

                _cogDisplay.Image = image;

                // 표시 모드에 따라 CogDisplay 뷰 조정
                if (_vm != null)
                {
                    if (_vm.IsFitMode)
                        _cogDisplay.Fit(true);
                    else if (_vm.IsOneToOne)
                        _cogDisplay.Zoom = 1.0;
                }
            });
        }

        // ── 리소스 정리 ───────────────────────────────────────────────────

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_vm != null)
            {
                _vm.ImageAcquired -= OnImageAcquired;
                _vm = null;
            }

            DataContextChanged -= OnDataContextChanged;
            Unloaded -= OnUnloaded;

            if (_cogDisplay != null)
            {
                CogDisplayHost.Child = null;
                _cogDisplay.Dispose();
                _cogDisplay = null;
            }
        }
    }
}
