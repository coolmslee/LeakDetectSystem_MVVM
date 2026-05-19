# WPF Style Guide

## 목적
이 문서는 `LeakDetectSystem_MVVM` 프로젝트에서 WPF 리소스, 스타일, startup-loaded view를 안정적으로 유지하기 위한 기준을 정의한다.

목표는 다음과 같다.

- 앱 startup 안정성 유지
- XAML parse/runtime 오류 방지
- 스타일 구조 단순화
- 공용 리소스 재사용성과 유지보수성 확보
- startup 경로와 일반 화면의 스타일 전략 분리

---

## 1. 핵심 원칙

### 1.1 Startup 경로는 보수적으로 유지한다
다음 파일 및 이들이 로드하는 모든 하위 컨트롤은 startup-critical 영역으로 간주한다.

- `MainWindow.xaml`
- `Views/Main/Controls/MainTabView.xaml`
- `Views/Main/Controls/MainTopDashboardView.xaml`
- `Views/Main/Controls/StationGroupView.xaml`
- `Views/Main/Controls/StationCardView.xaml`
- 앱 시작 직후 생성되는 모든 `UserControl`

이 영역에서는 스타일과 리소스 사용을 단순하게 유지해야 한다.

### 1.2 공용 스타일은 얇게 유지한다
공용 스타일은 다음과 같은 기본 속성 위주로만 구성한다.

- `FontSize`
- `FontWeight`
- `Padding`
- `Margin`
- `MinWidth`
- `MinHeight`
- `Width`
- `Height`
- `BorderThickness`
- `HorizontalAlignment`
- `VerticalAlignment`
- `Cursor`

가능하면 공용 스타일은 “기본 배치와 기본 표현”까지만 담당한다.

### 1.3 복잡한 스타일 기능은 제한적으로 사용한다
다음 기능은 startup-critical 영역에서 사용을 지양한다.

- `ControlTemplate`
- `TemplateBinding`
- `Style.Triggers`
- `DataTrigger`
- 다단계 `BasedOn`
- local resource style
- 복잡한 `StaticResource` 체인

### 1.4 동작 확인 후 공통화한다
새 UI는 다음 순서로 개발한다.

1. 먼저 직접 속성으로 구현한다.
2. 정상 동작과 렌더링을 확인한다.
3. 반복되는 속성만 공용 스타일로 추출한다.
4. 필요 시에만 브러시, trigger, template를 도입한다.

처음부터 스타일을 과도하게 추상화하지 않는다.

---

## 2. 리소스 구조 기준

### 2.1 리소스 파일 역할 분리
권장 역할은 다음과 같다.

- `Colors.xaml`
  - `Color`
  - `SolidColorBrush`
- `TextStyles.xaml`
  - `TextBlock` 중심 텍스트 스타일
- `ButtonStyles.xaml`
  - 버튼 스타일
- `BorderStyles.xaml`
  - 카드, 패널, 보더 공통 속성
- `StatusStyles.xaml`
  - 상태 표시 요소 스타일
- `DashboardStyles.xaml`
  - 대시보드 전용 스타일
- `Styles.xaml`
  - 전체 스타일 merge 진입점

### 2.2 병합 순서 규칙
`Styles.xaml`에서는 반드시 `Colors.xaml`이 먼저 병합되어야 한다.

예시:

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Colors.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/TextStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/ButtonStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/BorderStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/TabStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/MenuStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/StatusStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/DashboardStyles.xaml" />
    <ResourceDictionary Source="/LeakDetectSystem_MVVM;component/Views/Resources/Styles/DialogStyles.xaml" />
</ResourceDictionary.MergedDictionaries>
```

### 2.3 브러시 키 정의 위치
브러시 키는 반드시 `Colors.xaml`에만 정의한다.

예:
- `PrimaryBrush`
- `PrimaryDarkBrush`
- `BackgroundBrush`
- `PanelBackgroundBrush`
- `TextPrimaryBrush`
- `TextOnPrimaryBrush`
- `BorderBrush`
- `DividerBrush`

다른 스타일 사전에서 브러시를 새로 정의하거나 우회 정의하지 않는다.

---

## 3. 브러시 사용 규칙

### 3.1 허용 패턴
다음과 같은 직접 브러시 참조는 허용한다.

```xml
Background="{StaticResource BackgroundBrush}"
Foreground="{StaticResource TextPrimaryBrush}"
BorderBrush="{StaticResource DividerBrush}"
```

### 3.2 권장 사항
- 브러시 이름은 의미가 분명해야 한다.
- 색상 역할은 브러시 이름에 반영한다.
- startup 핵심 화면에서는 브러시 사용 개수를 최소화한다.

### 3.3 금지 또는 지양
다음 패턴은 지양한다.

- 브러시를 여러 단계로 간접 참조하는 구조
- 브러시 값이 trigger나 상속 체인을 통해서만 결정되는 구조
- startup-critical view에서 브러시가 없으면 바로 실패하는 템플릿 구조

---

## 4. 스타일 설계 규칙

### 4.1 권장 스타일 형태
권장 스타일은 다음과 같은 단순 명시형 스타일이다.

```xml
<Style x:Key="PrimaryButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="16,8" />
    <Setter Property="FontSize" Value="13" />
    <Setter Property="BorderThickness" Value="0" />
    <Setter Property="Cursor" Value="Hand" />
    <Setter Property="Background" Value="{StaticResource PrimaryBrush}" />
</Style>
```

### 4.2 스타일 이름 규칙
스타일 키는 역할이 드러나야 한다.

예:
- `PrimaryButtonStyle`
- `AccentButtonStyle`
- `HeaderTextStyle`
- `SubHeaderTextStyle`
- `CardStyle`
- `StatusIndicatorStyle`

모호한 범용 이름은 피한다.

### 4.3 `BasedOn` 사용 규칙
`BasedOn`은 가능하면 사용하지 않는다.
정말 필요하더라도 1단계까지만 허용한다.

허용 예:
- `SubtleButtonStyle` → `BasedOn="{StaticResource PrimaryButtonStyle}"`

지양 예:
- A → B → C → D 형태의 다단계 상속

### 4.4 Trigger 사용 규칙
Trigger는 다음 조건을 모두 만족할 때만 도입한다.

- 단순 setter로 해결되지 않는다.
- startup-critical 영역이 아니다.
- 해당 상태 변화가 UI 요구사항상 반드시 필요하다.
- 디버깅과 테스트가 가능한 수준으로 제한된다.

---

## 5. View 작성 규칙

### 5.1 Startup-critical view
startup-critical view에서는 다음 원칙을 따른다.

- 레이아웃은 view에서 명시적으로 정의한다.
- 스타일 의존은 최소화한다.
- local `<Style>` 사용을 피한다.
- `UserControl.Resources` 사용을 피한다.
- 고급 템플릿을 피한다.

### 5.2 반복형 UI
`ItemsControl`, `DataTemplate`, 카드형 반복 UI는 단순하게 유지한다.

권장:
- `Border`
- `Grid`
- `TextBlock`
- `Button`
- `Ellipse`

지양:
- 복잡한 nested style
- 여러 단계의 사용자 정의 컨트롤 중첩
- 템플릿 내부 trigger 과다 사용

### 5.3 하위 컨트롤 분리 기준
하위 컨트롤은 다음 기준을 만족해야 한다.

- 단독으로 렌더링해도 문제를 추적할 수 있어야 한다.
- 부모 스타일에 과도하게 의존하지 않아야 한다.
- 리소스 의존성이 분명해야 한다.

---

## 6. Startup-safe 규칙

### 6.1 특별 관리 대상
다음 영역은 “깨지면 앱 전체가 죽는 영역”으로 취급한다.

- `MainWindow`
- 메인 탭/대시보드
- station 카드/그룹
- title bar
- startup 직후 표시되는 모든 view

### 6.2 특별 관리 영역에서 금지
- local style
- inline trigger
- custom `ControlTemplate`
- 다단계 `BasedOn`
- 불명확한 resource chain

### 6.3 특별 관리 영역에서 허용
- 명시적인 `Grid`/`Border` 기반 레이아웃
- 기본 컨트롤 직접 사용
- 검증된 브러시 참조
- 단순 shared style

---

## 7. 점진적 스타일 확장 절차

새 스타일 또는 UI 변경은 아래 순서를 따른다.

### 7.1 1단계: 직접 구현
먼저 view에 직접 속성을 써서 UI를 만든다.

### 7.2 2단계: 동작 검증
앱 startup, 화면 진입, 바인딩, 렌더링이 정상인지 확인한다.

### 7.3 3단계: 공통화
반복 속성만 공용 스타일로 추출한다.

### 7.4 4단계: 장식 요소 추가
hover, pressed, selected 등 장식적 상태는 마지막에 추가한다.

---

## 8. 코드 리뷰 체크리스트

PR 리뷰 시 반드시 다음을 확인한다.

### 리소스
- 새 색상/브러시가 `Colors.xaml`에만 정의되었는가
- `Styles.xaml` merge 순서가 안전한가

### 스타일
- 이 스타일이 정말 공용화할 가치가 있는가
- `BasedOn` 없이 표현 가능한가
- trigger/template 없이도 가능한가
- startup 영역에서 과도한 스타일 의존을 만들지 않는가

### 뷰
- local style이 추가되지 않았는가
- startup-critical view가 복잡해지지 않았는가
- 하위 컨트롤이 디버깅 가능한 구조인가
- 브러시 참조가 과도하게 늘어나지 않았는가

---

## 9. 장애 대응 규칙

WPF XAML parse/runtime 오류가 발생하면 다음 순서로 대응한다.

1. startup 경로인지 확인한다.
2. 새로 추가한 style/resource/view를 우선 의심한다.
3. local style, trigger, template를 먼저 제거한다.
4. shared style 의존을 줄여 재현 범위를 좁힌다.
5. child control을 하나씩 제거하여 원인을 분리한다.
6. 문제를 해결한 뒤에만 스타일을 다시 공통화한다.

---

## 10. 결론

이 프로젝트의 스타일 기준은 다음 한 줄로 요약한다.

**Startup 경로는 단순하고 명시적으로, 공용 스타일은 얇고 예측 가능하게 유지한다.**

예쁘고 복잡한 스타일보다, 안정적으로 뜨고 유지보수 가능한 구조가 우선이다.