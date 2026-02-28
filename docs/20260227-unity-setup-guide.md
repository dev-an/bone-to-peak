# Unity 설치 및 프로젝트 초기화 가이드

> 작성일: 2026-02-27

---

## 1. Unity Hub 설치

### macOS

```bash
# Homebrew로 설치
brew install --cask unity-hub
```

또는 [Unity Hub 다운로드 페이지](https://unity.com/download)에서 직접 다운로드.

### 설치 확인

Unity Hub를 실행하고, Unity 계정으로 로그인한다.

---

## 2. Unity 6 LTS 설치

1. Unity Hub 실행
2. 좌측 메뉴에서 **Installs** 클릭
3. **Install Editor** 버튼 클릭
4. **Unity 6 (6000.x LTS)** 선택
5. 추가 모듈 선택:
    - **Visual Studio Code Editor** (VS Code 사용 시)
    - **Windows Build Support (Mono)** (Steam PC 빌드용, 필요시)
6. **Install** 클릭

> Unity 6 LTS 버전 확인: `6000.0.x` 형태의 최신 LTS 버전을 선택한다.

---

## 3. 기존 리포지토리를 Unity 프로젝트로 초기화

이 리포지토리에는 이미 `Assets/` 디렉토리와 설정 파일이 준비되어 있다.
Unity Hub에서 새 프로젝트를 생성하되, 이 리포지토리 경로를 사용한다.

### 방법 A: Unity Hub에서 직접 생성 (권장)

1. Unity Hub → **Projects** → **New Project**
2. 템플릿: **2D (URP)** 선택
3. **Project name**: `bone-to-peak`
4. **Location**: 이 리포지토리의 **상위 디렉토리** 선택
    - 예: 리포지토리가 `~/Workspace/bone-to-peak`이면, Location은 `~/Workspace`
5. **Create Project** 클릭

> Unity가 기존 `Assets/` 폴더를 인식하고, `Library/`, `Packages/`, `ProjectSettings/` 등을 자동 생성한다.

### 방법 B: 기존 프로젝트로 열기

만약 이미 Unity 프로젝트 파일(`ProjectSettings/` 등)이 있는 경우:

1. Unity Hub → **Projects** → **Open** → **Add project from disk**
2. 이 리포지토리 루트 디렉토리 선택
3. Unity가 프로젝트를 열고 `Library/`를 재생성

---

## 4. Unity 프로젝트 초기 설정

프로젝트가 열리면 다음 설정을 확인한다:

### Version Control 설정

1. **Edit → Project Settings → Editor**
2. **Version Control**:
    - Mode: **Visible Meta Files**
3. **Asset Serialization**:
    - Mode: **Force Text**

> 이 설정은 Git에서 Unity 프로젝트를 올바르게 관리하기 위해 필수적이다.

### 렌더 파이프라인 확인

1. **Edit → Project Settings → Graphics**
2. **Scriptable Render Pipeline Settings**에 URP Asset이 할당되어 있는지 확인

---

## 5. VS Code 연동

1. Unity에서 **Edit → Preferences → External Tools**
2. **External Script Editor**: Visual Studio Code 선택
3. **Generate .csproj files** 옵션 활성화:
    - [x] Embedded packages
    - [x] Local packages

이후 Unity에서 C# 스크립트를 더블클릭하면 VS Code가 열린다.

---

## 6. 초기화 후 확인 체크리스트

- [ ] Unity Hub에서 프로젝트가 정상적으로 열리는가?
- [ ] 2D URP 템플릿이 적용되어 있는가?
- [ ] `Assets/` 하위 디렉토리 구조가 유지되는가?
- [ ] `Library/`, `Temp/` 등이 `.gitignore`에 의해 제외되는가?
- [ ] VS Code에서 C# IntelliSense가 동작하는가?
- [ ] Version Control 모드가 "Visible Meta Files"인가?
- [ ] Asset Serialization 모드가 "Force Text"인가?
