# LeakDetectSystem_MVVM

WPF MVVM 패턴으로 리팩토링된 누설 감지 시스템 프로젝트입니다.

---

## 📁 폴더 구조

```
LeakDetectSystem_MVVM/
├── LeakDetectSystem_MVVM.csproj   # WPF 프로젝트 파일 (.NET 8)
├── App.xaml                        # 앱 리소스(MergedDictionaries) 로드
├── App.xaml.cs                     # 앱 진입점 - MainWindow + ViewModel 주입
│
├── Models/                         # 순수 데이터 모델 (비즈니스 엔티티)
│   └── LeakInfoModel.cs            # 누설 감지 정보 데이터 모델
│
├── ViewModels/                     # UI 상태 + 로직 (View에 바인딩)
│   ├── Base/
│   │   └── ViewModelBase.cs        # INotifyPropertyChanged + SetProperty 헬퍼
│   ├── MainWindowViewModel.cs      # 메인 창 VM (탭 관리, 앱 레벨 명령)
│   ├── MainTabViewModel.cs         # 모니터링 탭 VM (스테이션 목록, 모니터링 토글)
│   ├── SettingTabViewModel.cs      # 설정 탭 VM (장치/임계값 설정)
│   └── StationViewModel.cs         # 개별 스테이션 패널 VM
│
├── Commands/                       # ICommand 구현체
│   ├── RelayCommand.cs             # 동기 커맨드 (RelayCommand, RelayCommand<T>)
│   └── AsyncRelayCommand.cs        # 비동기 커맨드 (실행 중 중복 방지)
│
├── Services/                       # 서비스 인터페이스 + 구현체
│   ├── INavigationService.cs       # 화면 이동 서비스 인터페이스
│   ├── IDialogService.cs           # 다이얼로그 서비스 인터페이스
│   └── DialogService.cs            # IDialogService WPF 구현체
│
└── Views/
    ├── Resources/                  # 전역 리소스 사전
    │   ├── Colors.xaml             # 색상 정의 (Color + SolidColorBrush)
    │   └── Styles.xaml             # 컨트롤 스타일 정의
    └── Main/
        ├── MainWindow.xaml         # 메인 창 (탭 헤더 + ContentControl)
        ├── MainWindow.xaml.cs      # 코드-비하인드 최소화 (InitializeComponent만)
        └── Controls/               # 탭별 UserControl
            ├── MainTabView.xaml    # 모니터링 탭 View (ItemsControl + StationView)
            ├── MainTabView.xaml.cs
            ├── SettingTabView.xaml # 설정 탭 View
            ├── SettingTabView.xaml.cs
            ├── StationView.xaml    # 개별 스테이션 패널 UserControl
            └── StationView.xaml.cs
```

---

## 🏗️ 아키텍처 패턴

### MVVM 레이어

| 레이어 | 역할 | 디렉터리 |
|---|---|---|
| **Model** | 순수 데이터 구조 (비즈니스 엔티티) | `Models/` |
| **ViewModel** | UI 상태 + 로직, INotifyPropertyChanged | `ViewModels/` |
| **View** | XAML UI, DataContext 바인딩 | `Views/` |
| **Command** | ICommand 구현 (RelayCommand) | `Commands/` |
| **Service** | 외부 의존성 추상화 | `Services/` |

### DataContext 주입 방식

`App.xaml.cs`의 `OnStartup`에서 ViewModel을 생성하여 MainWindow에 주입합니다.

```csharp
// App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    var viewModel = new MainWindowViewModel();
    var mainWindow = new MainWindow { DataContext = viewModel };
    mainWindow.Show();
}
```

### ViewModel → View 자동 연결 (DataTemplate)

`MainWindow.xaml`에 DataTemplate을 등록하면, `ContentControl`이 현재 ViewModel 타입에 맞는 View를 자동으로 렌더링합니다.

```xml
<!-- MainWindow.xaml -->
<Window.Resources>
    <DataTemplate DataType="{x:Type vm:MainTabViewModel}">
        <controls:MainTabView />
    </DataTemplate>
    <DataTemplate DataType="{x:Type vm:SettingTabViewModel}">
        <controls:SettingTabView />
    </DataTemplate>
</Window.Resources>

<!-- SelectedTab이 바뀌면 DataTemplate에 의해 올바른 View가 렌더링됨 -->
<ContentControl Content="{Binding SelectedTab}" />
```

---

## ➕ 새 View / ViewModel 추가 방법

### 1. 새 탭 추가 예시 (`ReportTab` 추가)

**① Model 추가** (필요 시)
```
Models/ReportModel.cs
```

**② ViewModel 추가**
```
ViewModels/ReportTabViewModel.cs
```
```csharp
public class ReportTabViewModel : ViewModelBase
{
    // 바인딩 속성 + Commands 정의
}
```

**③ View 추가**
```
Views/Main/Controls/ReportTabView.xaml
Views/Main/Controls/ReportTabView.xaml.cs
```

**④ MainWindowViewModel에 탭 등록**
```csharp
// ViewModels/MainWindowViewModel.cs
public ReportTabViewModel ReportTab { get; } = new();
```

**⑤ MainWindow에 DataTemplate 등록**
```xml
<!-- Views/Main/MainWindow.xaml -->
<DataTemplate DataType="{x:Type vm:ReportTabViewModel}">
    <controls:ReportTabView />
</DataTemplate>
```

**⑥ 탭 버튼 추가**
```xml
<Button Content="리포트"
        Command="{Binding NavigateCommand}"
        CommandParameter="report" />
```

**⑦ NavigateTo 메서드에 케이스 추가**
```csharp
// ViewModels/MainWindowViewModel.cs
private void NavigateTo(string? viewName)
{
    SelectedTab = viewName switch
    {
        "main"    => MainTab,
        "setting" => SettingTab,
        "report"  => ReportTab,   // 추가
        _         => SelectedTab
    };
}
```

---

### 2. Command 사용 예시

```csharp
// ViewModel에서
public RelayCommand MyCommand { get; }

public MyViewModel()
{
    MyCommand = new RelayCommand(DoSomething, CanDoSomething);
}

private void DoSomething() { /* ... */ }
private bool CanDoSomething() => !IsBusy;
```

```xml
<!-- View XAML에서 -->
<Button Content="실행" Command="{Binding MyCommand}" />
```

---

### 3. Resources 확장

`Views/Resources/`에 새 ResourceDictionary를 추가하고 `App.xaml`에 등록합니다.

```xml
<!-- App.xaml -->
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="Views/Resources/Colors.xaml" />
    <ResourceDictionary Source="Views/Resources/Styles.xaml" />
    <ResourceDictionary Source="Views/Resources/MyNewDict.xaml" />  <!-- 추가 -->
</ResourceDictionary.MergedDictionaries>
```

---

## 🔨 빌드 및 실행

```bash
# 빌드
dotnet build

# 실행 (Windows 필요)
dotnet run
```

> **요구사항**: .NET 8 SDK, Windows OS (WPF는 Windows 전용)

---

## 📖 참고 자료

- [WPF MVVM 패턴 개요 (Microsoft Docs)](https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/data/data-binding-overview)
- [INotifyPropertyChanged (Microsoft Docs)](https://learn.microsoft.com/ko-kr/dotnet/api/system.componentmodel.inotifypropertychanged)
- [RelayCommand 패턴](https://learn.microsoft.com/ko-kr/dotnet/communitytoolkit/mvvm/relaycommand)
