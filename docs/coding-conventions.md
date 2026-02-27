# Coding Conventions (C# / Unity)

## 네이밍

| 대상             | 규칙              | 예시                              |
| ---------------- | ----------------- | --------------------------------- |
| 클래스 / 구조체  | PascalCase        | `PlayerController`, `WaveManager` |
| 퍼블릭 메서드    | PascalCase        | `TakeDamage()`, `SpawnMinion()`   |
| 프라이빗 메서드  | PascalCase        | `CalculateHealth()`               |
| 퍼블릭 프로퍼티  | PascalCase        | `MaxHealth`, `MoveSpeed`          |
| 프라이빗 필드    | _camelCase        | `_currentHealth`, `_moveSpeed`    |
| 로컬 변수        | camelCase         | `spawnPoint`, `enemyCount`        |
| 상수             | PascalCase        | `MaxWaveCount`, `DefaultSpeed`    |
| 인터페이스       | I + PascalCase    | `IDamageable`, `IPoolable`        |
| 열거형           | PascalCase (단수) | `MinionType`, `FormationType`     |
| ScriptableObject | SO 접미사         | `SkillDataSO`, `MinionStatsSO`    |

## 파일 구조

- 하나의 파일에 하나의 클래스
- 파일명 = 클래스명 (예: `PlayerController.cs`)
- 네임스페이스: `BoneToPeak.{모듈}` (예: `BoneToPeak.Core`, `BoneToPeak.Player`)

## Scripts 디렉토리

```
Assets/Scripts/
├── Core/           # GameManager, ObjectPool, EventSystem, 싱글톤
├── Player/         # Necromancer 이동, 입력, 상태
├── Minions/        # 소환수 AI, 스폰, 포메이션, 종류별 행동
├── Enemies/        # 적 AI, 웨이브 시스템, 스포너
├── Skills/         # ScriptableObject 기반 스킬 데이터 및 로직
└── UI/             # HUD, 메뉴, 팝업
```

## 코드 스타일

- `#region` 사용 자제 — 코드가 길어지면 클래스 분리
- `SerializeField`로 Inspector 노출, 퍼블릭 필드 지양
- `RequireComponent` 어트리뷰트 적극 활용
- 매직 넘버 대신 상수 또는 ScriptableObject 사용

## MonoBehaviour 메서드 순서

```csharp
// 1. Unity 콜백
Awake()
OnEnable()
Start()
Update() / FixedUpdate()
OnDisable()
OnDestroy()

// 2. 퍼블릭 메서드
// 3. 프라이빗 메서드
```
