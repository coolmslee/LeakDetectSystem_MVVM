# Dialog Refactoring Guidelines

## 목적
`Views/Dialogs` 아래의 XAML/Code-behind/ViewModel 구성을 일관된 MVVM 규칙으로 정리한다.

현재 프로젝트의 Dialog는 다음 문제가 혼재되어 있다.

- View가 XAML에서 ViewModel을 직접 생성한다.
- 어떤 Dialog는 Command를 쓰고, 어떤 Dialog는 Click event를 사용한다.
- 공통 다이얼로그 스타일이 부족하다.
- 단순 설정 팝업과 실제 도구 창 수준의 화면이 같은 분류에 섞여 있다.

이 문서는 `LeakDetectSystem_MVVM`에서 Dialog를 어떻게 구성할지에 대한 기준을 정의한다.

---

## 적용 대상
- `Views/Dialogs/*.xaml`
- `Views/Dialogs/*.xaml.cs`
- `ViewModels/Dialogs/*`
- `Services/DialogService.cs`
- `Services/IDialogService.cs`

---

## 핵심 원칙

### 1) View는 ViewModel을 직접 생성하지 않는다
다음 패턴은 사용하지 않는다.

```xml
<Window.DataContext>
    <vm:CameraDialogViewModel/>
</Window.DataContext>
```

이유:
- View가 특정 ViewModel 구현에 직접 의존하게 된다.
- 생성자 주입이 어려워진다.
- 테스트가 어려워진다.
- DialogService와 역할이 겹친다.

원칙:
- ViewModel은 View 밖에서 생성한다.
- 생성된 ViewModel은 DialogService 또는 상위 호출자에서 주입한다.

---

### 2) Dialog 생성 책임은 서비스 계층으로 모은다
Dialog를 여는 책임은 가능한 `DialogService`가 가진다.

권장 흐름:
1. 상위 ViewModel이 Dialog open 요청
2. DialogService가 Window 생성
3. DialogService가 ViewModel을 DataContext로 주입
4. Dialog 표시
5. 결과 반환 또는 후속 처리

즉, ViewModel은 “창을 어떻게 만들지”가 아니라
“어떤 다이얼로그를 왜 열어야 하는지”만 알아야 한다.

---

### 3) 사용자 액션은 Command를 우선 사용한다
다음은 ViewModel에서 처리한다.

- 저장
- 확인
- 취소 의도
- 값 검증
- 적용 여부 판단

다음은 View 계층에 남을 수 있다.

- 실제 `Window.Close()`
- 포커스 이동
- 순수 시각 동작

즉:
- 비즈니스 의도는 ViewModel
- Window 자체 제어는 최소 범위에서 View

---

### 4) Code-behind는 최소화하되 완전 금지는 아니다
WPF에서 Dialog 닫기 자체는 View 개념과 가깝기 때문에,
Code-behind를 완전히 금지할 필요는 없다.

하지만 Code-behind는 아래에만 사용한다.

허용:
- `Close()` 호출
- `DialogResult` 설정
- Window Loaded 같은 순수 View 이벤트 처리
- DataContext 기반의 Window 전용 연결 처리

금지:
- 검증 로직
- 저장 로직
- 도메인 로직
- 서비스 직접 호출
- 상태 계산

---

### 5) Dialog와 Tool Window를 구분한다
모든 팝업 화면을 `Dialog`로 취급하지 않는다.

#### Dialog
특징:
- 짧은 입력
- 설정 변경
- 확인/취소 중심
- 모달 성격

예:
- Camera 설정
- Light 설정
- Grab 설정
- Model 설정

#### Tool Window
특징:
- 기능량이 많음
- 지속적으로 열어두고 사용 가능
- 모니터링/조작/디버깅 목적
- 복합 레이아웃

예:
- PLC 통신 화면
- Log 조회 화면(규모에 따라)

이 기준에 따라 필요 시 다음을 검토한다.
- `PlcDialog` → `PlcToolWindow`
- `LogDialog` → `LogWindow` 또는 유지 여부 재검토

---

## 구조 규칙

### View 위치
```text
Views/
  Dialogs/
    Settings/
    Monitoring/
    Tools/
```

권장 예시:
- `Views/Dialogs/Settings/CameraDialog.xaml`
- `Views/Dialogs/Settings/GrabDialog.xaml`
- `Views/Dialogs/Settings/LightDialog.xaml`
- `Views/Dialogs/Settings/ModelDialog.xaml`
- `Views/Dialogs/Monitoring/LogWindow.xaml`
- `Views/Dialogs/Tools/PlcToolWindow.xaml`

---

### ViewModel 위치
```text
ViewModels/
  Dialogs/
    Settings/
    Monitoring/
    Tools/
```

View와 ViewModel은 분류 체계를 맞춘다.

---

## DataContext 규칙

### 금지 예시
```xml
<Window.DataContext>
    <vm:GrabDialogViewModel/>
</Window.DataContext>
```

### 권장 방식
- Window 생성 후 외부에서 `DataContext` 설정
- 또는 DialogService 내부에서 생성/주입

예시 개념:
1. `var vm = new CameraDialogViewModel(...)`
2. `var view = new CameraDialog()`
3. `view.DataContext = vm`
4. `view.ShowDialog()`

---

## 닫기 규칙

### 현재 문제
일부 Dialog는 `CloseButton_Click`를 사용하고,
일부는 `Command`를 사용한다.

이 프로젝트에서는 닫기 패턴을 통일한다.

### 권장 패턴
- `ConfirmCommand`
- `CancelCommand`
- `CloseRequested` 이벤트 또는 상태
- DialogService/Window code-behind가 실제 `Close()` 수행

### 최소 허용 패턴
단순 닫기 버튼만 있는 경우:
- 버튼은 ViewModel의 `CloseCommand` 또는 `CancelCommand`에 바인딩
- Window는 ViewModel이 발생시키는 close signal을 구독하여 닫는다

---

## 스타일 규칙
Dialog XAML에서는 다음을 직접 반복 작성하지 않도록 한다.

- 동일한 Margin / Padding
- 동일한 헤더 FontSize / FontWeight
- 동일한 BorderBrush / BorderThickness
- 동일한 버튼 크기와 정렬

이 값들은 `Views/Resources/Styles/DialogStyles.xaml`에 둔다.

권장 공통 스타일:
- `DialogWindowStyle`
- `DialogTitleTextStyle`
- `DialogSectionBorderStyle`
- `DialogLabelTextStyle`
- `DialogInputTextBoxStyle`
- `DialogFooterPanelStyle`
- `DialogPrimaryButtonStyle`
- `DialogSecondaryButtonStyle`

---

## 파일별 적용 방향

### CameraDialog / GrabDialog / LightDialog / ModelDialog
방향:
- XAML 내 ViewModel 직접 생성 제거
- 반복 레이아웃 스타일 공통화
- 닫기 버튼 패턴 통일
- 설정형 Dialog로 분류 유지

### LogDialog
방향:
- 공통 스타일 일부 적용
- 단순 Dialog인지 별도 Window인지 재평가
- 하드코딩된 색상과 크기 정리

### PlcDialog
방향:
- 단순 Dialog가 아니라 Tool Window 성격으로 재분류 검토
- 하드코딩 리소스를 스타일/브러시로 이동
- 가능하면 이름도 역할에 맞게 정리

---

## 네이밍 규칙

### View
- 설정용 모달 창: `SomethingDialog`
- 독립 도구 창: `SomethingToolWindow` 또는 `SomethingWindow`

### ViewModel
- `SomethingDialogViewModel`
- `SomethingToolWindowViewModel`

이름은 역할을 드러내야 한다.

---

## 마이그레이션 순서

### 1단계
- `Window.DataContext` 직접 생성 제거
- DialogService 또는 외부 주입 방식으로 전환

### 2단계
- `CloseButton_Click` 제거 또는 최소화
- 닫기 패턴 일관화

### 3단계
- Dialog 공통 스타일 추출
- 하드코딩된 시각 속성 제거

### 4단계
- Dialog / Monitoring / Tool Window 재분류
- 필요 시 파일명 정리

---

## 금지 규칙
다음은 지양 또는 금지한다.

- Dialog XAML에서 ViewModel 직접 `new`
- Code-behind에 저장/검증/비즈니스 로직 작성
- 다이얼로그마다 서로 다른 종료 패턴 사용
- 공통 스타일이 가능한데도 개별 속성 반복 작성
- Tool 수준 화면을 무조건 Dialog로 유지

---

## 이 프로젝트에서 기대하는 효과
이 규칙을 적용하면 다음 효과를 얻는다.

- Dialog 생성 방식 일관화
- MVVM 책임 경계 명확화
- 테스트/확장 용이성 향상
- 반복 XAML 감소
- 설정창과 운영 도구창의 역할 분리
