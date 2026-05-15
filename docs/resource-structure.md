# Resource Folder Structure Guide

## 목적
`Views/Resources/Styles.xaml`에 다양한 성격의 스타일이 한 파일에 몰려 있어,
스타일 검색, 수정 범위 파악, 재사용, 충돌 관리가 어려워지는 문제를 해결한다.

이 문서는 `LeakDetectSystem_MVVM` 프로젝트에서 사용할 리소스 분리 기준과 폴더 구조를 정의한다.

---

## 현재 문제
현재 `Views/Resources/Styles.xaml`에는 다음 성격의 스타일이 함께 존재한다.

- 텍스트 스타일
- 버튼 스타일
- 패널/카드 스타일
- 탭 스타일
- 메뉴 스타일
- 상태 표시 스타일
- 대시보드 전용 스타일
- DisplayMode 버튼 스타일
- DIO Toggle 스타일

이 구조는 초기 구현에는 편하지만, 기능이 늘어날수록 다음 문제가 생긴다.

1. 특정 스타일의 위치를 찾기 어렵다.
2. 공통 스타일과 화면 전용 스타일이 섞인다.
3. 수정 영향 범위를 파악하기 어렵다.
4. 여러 작업자가 동시에 수정할 때 충돌 가능성이 높다.
5. 비슷한 스타일의 중복 생성이 쉬워진다.

---

## 기본 원칙

### 1) 색상과 스타일을 분리한다
- 색상/브러시 토큰은 `Colors.xaml`에 둔다.
- 컨트롤 스타일은 `Styles/` 하위 파일로 분리한다.

### 2) 공통 스타일과 화면 전용 스타일을 분리한다
- 여러 View에서 공통으로 사용하는 스타일은 공통 스타일 파일에 둔다.
- 특정 화면/기능에서만 사용하는 스타일은 기능 전용 스타일 파일에 둔다.

### 3) 진입점은 하나로 유지한다
- 외부에서는 가능하면 하나의 ResourceDictionary만 merge 하도록 유지한다.
- 내부적으로만 여러 파일로 분리한다.

### 4) 기존 키 이름은 가능한 유지한다
- 대규모 XAML 수정 비용을 줄이기 위해 기존 resource key는 가능한 유지한다.
- 이름 변경은 의미가 명확해지는 경우에만 수행한다.

---

## 권장 폴더 구조

```text
Views/
  Resources/
    Colors.xaml
    Styles.xaml
    Styles/
      TextStyles.xaml
      ButtonStyles.xaml
      BorderStyles.xaml
      TabStyles.xaml
      MenuStyles.xaml
      StatusStyles.xaml
      DashboardStyles.xaml
      DialogStyles.xaml
```

---

## 파일별 역할

### Colors.xaml
역할:
- Color
- SolidColorBrush
- 테마 토큰

예:
- `PrimaryBrush`
- `PrimaryLightBrush`
- `TextPrimaryBrush`
- `PanelBackgroundBrush`

주의:
- 스타일 정의는 두지 않는다.
- 시각 토큰만 관리한다.

---

### Styles.xaml
역할:
- 실제 스타일 정의를 두는 파일이 아니라,
- 내부 스타일 파일들을 모아주는 집계용 ResourceDictionary 역할만 담당한다.

예시 구조:
- `Colors.xaml`
- `Styles/TextStyles.xaml`
- `Styles/ButtonStyles.xaml`
- `Styles/BorderStyles.xaml`
- `Styles/TabStyles.xaml`
- `Styles/MenuStyles.xaml`
- `Styles/StatusStyles.xaml`
- `Styles/DashboardStyles.xaml`
- `Styles/DialogStyles.xaml`

주의:
- 새로운 스타일을 직접 `Styles.xaml`에 추가하지 않는다.
- 반드시 적절한 하위 스타일 파일에 추가한다.

---

### TextStyles.xaml
역할:
- `TextBlock` 중심의 공통 텍스트 스타일

예:
- `HeaderTextStyle`
- `SubHeaderTextStyle`
- `BodyTextStyle`
- `SecondaryTextStyle`

---

### ButtonStyles.xaml
역할:
- `Button`, `ToggleButton` 공통 스타일

예:
- `PrimaryButtonStyle`
- `AccentButtonStyle`
- `DisplayModeButtonStyle`
- `DioOutputToggleStyle`

주의:
- 버튼 계열 스타일이 많아질 경우 `ToggleButtonStyles.xaml`로 추가 분리 가능하다.

---

### BorderStyles.xaml
역할:
- `Border`, `Rectangle`, 카드/패널 스타일

예:
- `CardStyle`
- 공통 패널 래퍼 스타일
- 구분선 스타일

---

### TabStyles.xaml
역할:
- `TabControl`, `TabItem` 관련 스타일

예:
- `MainTabControlStyle`
- `MainTabItemStyle`

---

### MenuStyles.xaml
역할:
- 메뉴/타이틀바 메뉴 관련 스타일

예:
- `TitleBarMenuStyle`
- `TitleBarTopMenuItemStyle`
- `TitleBarSubMenuItemStyle`
- `TitleBarMenuButtonStyle`
- `TitleBarExitButtonStyle`

---

### StatusStyles.xaml
역할:
- 연결상태, 상태 점, 상태 라벨, divider 등 상태 표현 스타일

예:
- `StatusIndicatorStyle`
- `ConnectionIndicatorStyle`
- `ConnectionLabelStyle`
- `ConnectionDividerStyle`

---

### DashboardStyles.xaml
역할:
- 메인 대시보드처럼 특정 기능 영역에 속한 스타일

예:
- `DashboardCellStyle`
- `DashboardCellHeaderStyle`
- `DashboardHeaderTextStyle`
- `DashboardCountTextStyle`
- `DashboardToggleButtonStyle`

주의:
- 다른 화면에서 재사용되지 않는 기능 전용 스타일을 여기에 둔다.

---

### DialogStyles.xaml
역할:
- 다이얼로그 공통 스타일
- 다이얼로그 헤더, 본문 Border, 버튼 배치, 입력 라벨/필드 스타일

예:
- `DialogTitleTextStyle`
- `DialogContainerBorderStyle`
- `DialogActionButtonStyle`
- `DialogLabelTextStyle`
- `DialogInputTextBoxStyle`

주의:
- 각 Dialog에서 반복되는 Margin, Padding, FontSize, BorderBrush 설정을 여기로 올린다.

---

## 스타일 추가 규칙

### 새 스타일을 추가할 때
1. 먼저 기존 스타일 재사용 가능 여부를 확인한다.
2. 공통인지 화면 전용인지 판단한다.
3. 공통이면 적절한 공통 스타일 파일에 추가한다.
4. 특정 화면 전용이면 기능 전용 스타일 파일에 추가한다.
5. `Styles.xaml`에는 merge entry만 추가한다.

### 스타일 이름 규칙
- `[역할][Control]Style` 형태를 우선 사용한다.
- 예:
  - `DialogTitleTextStyle`
  - `PrimaryButtonStyle`
  - `DashboardCellStyle`

### 화면 이름을 스타일 이름에 넣는 경우
- 다른 화면에서 재사용 가능성이 거의 없을 때만 허용한다.
- 예:
  - `DashboardCellStyle`

---

## 적용 순서 권장안

### 1단계
- 기존 `Styles.xaml`를 유지한 채 내부 파일만 분리한다.
- 외부 merge 방식은 바꾸지 않는다.

### 2단계
- Dialog 공통 스타일을 `DialogStyles.xaml`로 추출한다.
- Dialog XAML에서 반복 속성을 점진적으로 제거한다.

### 3단계
- 화면 전용 스타일과 공통 스타일을 다시 검토하여 중복을 정리한다.

---

## 금지 규칙
다음은 지양하거나 금지한다.

- `Styles.xaml`에 직접 새 스타일 누적 추가
- 색상값 하드코딩 반복 사용
- 공통 스타일 파일에 특정 화면 전용 스타일 무분별하게 추가
- 같은 역할의 스타일을 다른 이름으로 중복 생성

---

## 이 프로젝트에서 기대하는 효과
이 구조를 적용하면 다음 효과를 기대할 수 있다.

- 스타일 탐색 속도 향상
- 수정 범위 예측 가능성 증가
- 공통화 수준 향상
- Dialog/대시보드 등 기능 영역별 유지보수 편의성 향상
- 이후 MVVM 리팩토링과 함께 View 정리가 쉬워짐
