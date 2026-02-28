# Phase 3 구현 가이드

> 이 문서는 Phase 3의 6개 시스템에 대한 구현 순서, 의존성, 코드 구조를 정리한다.
> 각 시스템의 상세 기획은 개별 기획 문서를 참고한다.

---

## Phase 3 전체 로드맵

### 구현 순서 (의존성 기반)

```
Step 1 (독립 구현 가능)
├── 1A. 포메이션 시스템 (Guard 상태, 3가지 포메이션)
├── 1B. 플레이어 기본 공격 (지팡이/소울 애로우)
└── 1C. 경험치/레벨업 시스템 (경험치 구슬, 레벨업 UI)

Step 2 (Step 1 완료 후)
├── 2A. Refine 합성 시스템 (Step 1A 포메이션 필요)
└── 2B. Explode 시체 폭발 (독립적이지만 Corpse 시스템 확장)

Step 3 (Step 2 완료 후)
├── 3A. 엘리트 적 (기사, 마법사, 암살자)
└── 3B. 보스 시스템 (성기사 → 드래곤 → 대마법사 → 용사)

Step 4 (Step 3 완료 후)
└── 4A. UI/HUD 통합 (모든 시스템의 이벤트를 UI에 연결)
```

> Step 1의 3개 시스템은 **병렬 개발 가능**하다.

---

## Step 1A. 포메이션 시스템

> 기획 문서: [formation-system.md](./formation-system.md), [minion-system.md](./minion-system.md)

### 구현 범위

1. `FormationManager` — 포메이션 전환 관리, 슬롯 좌표 계산
2. `MinionBase` 확장 — Guard 상태 추가, 포메이션 보너스/패널티 적용
3. `FlockingSystem` — Boids 알고리즘 + Spatial Hashing
4. 입력 연결 — Q 키로 포메이션 순환 전환

### 코드 구조

```
Assets/Scripts/
├── Minions/
│   ├── MinionBase.cs              ← 수정: Guard 상태 추가, 포메이션 보너스
│   ├── MinionState.cs             ← 수정: Guard 추가
│   ├── FormationManager.cs        ← 신규: 포메이션 타입 관리, 슬롯 계산
│   ├── FormationType.cs           ← 신규: enum (Phalanx, Charge, Circular)
│   ├── FormationSlotCalculator.cs ← 신규: 3가지 포메이션별 슬롯 좌표 공식
│   └── FlockingSystem.cs          ← 신규: Boids + Spatial Hashing
```

### 핵심 구현 사항

**FormationManager (싱글톤)**:

- `CurrentFormation`: 현재 활성 포메이션 타입
- `SwitchFormation()`: Q 키 입력 처리, 쿨타임 3초 관리
- `GetSlotPosition(int index, int totalMinions)`: 미니언 인덱스 → 월드 좌표
- `IsTransitioning`: 전환 중 여부 (1초간 true)
- 이벤트: `OnFormationChanged(FormationType)` → GameEvents에 추가

**MinionBase 수정**:

- `MinionState.Guard` 추가
- Guard 진입 조건: `CurrentFormation == Phalanx && 적이 접근 중`
- Guard 동작: 슬롯 위치 고수, 접근 적만 공격 (이탈 없음)
- 포메이션 보너스 적용: `ApplyFormationModifiers()` — 공격력, 방어력, 속도 등 ScriptableObject 스탯에 배율 적용

**FlockingSystem**:

- 0.2초 간격 코루틴 (랜덤 오프셋 0~0.1초)
- `SpatialHash` 클래스: 셀 크기 3.0, 인접 9셀 탐색
- `CalculateFlockingForce()`: Separation(1.5) + Alignment(0.5) + Cohesion(0.8) + FormationTarget(2.0)
- 50마리 초과 시 Alignment 가중치 0

### 기존 코드 변경점

| 파일                  | 변경 내용                                                          |
| --------------------- | ------------------------------------------------------------------ |
| `GameEvents.cs`       | `OnFormationChanged` 이벤트 추가                                   |
| `MinionState.cs`      | `Guard` 상태 추가                                                  |
| `MinionBase.cs`       | Guard 상태 로직, FlockingSystem 연동, 현재 Golden Spiral 로직 대체 |
| `PlayerController.cs` | Q 키 입력 바인딩 (InputSystem Action 추가)                         |

### 테스트 기준

- [ ] Q 키로 Phalanx → Charge → Circular 순환 전환
- [ ] 각 포메이션에서 미니언이 올바른 슬롯 위치로 이동
- [ ] Phalanx에서 적 접근 시 Guard 상태 전환
- [ ] 포메이션 전환 쿨타임 3초 작동
- [ ] 전환 중 1초간 보너스 미적용
- [ ] 미니언 간 겹침 없이 자연스러운 무리 이동 (Flocking)

---

## Step 1B. 플레이어 기본 공격

> 기획 문서: [player-necromancer.md](./player-necromancer.md)

### 구현 범위

1. `PlayerAttack` — 기본 공격 시스템 (해골 지팡이 / 영혼 화살)
2. 투사체 시스템 (영혼 화살용)
3. 히트박스 시스템 (지팡이용 부채꼴 판정)

### 코드 구조

```
Assets/Scripts/
├── Player/
│   ├── PlayerAttack.cs           ← 신규: 좌클릭 공격, 무기 타입 분기
│   ├── PlayerAttackType.cs       ← 신규: enum (Staff, SoulArrow)
│   └── PlayerCombat.cs           ← 기존 유지 (체력, Corpse, 미니언 슬롯)
├── Core/
│   ├── Projectile.cs             ← 신규: 투사체 기반 (IPoolable)
│   └── HitboxMelee.cs            ← 신규: 부채꼴/원형 근접 판정
```

### 핵심 구현 사항

**PlayerAttack**:

- 런 시작 시 무기 선택 (UI 또는 기본값)
- 좌클릭: `Fire()` → 무기 타입에 따라 분기
- 쿨타임 관리: 지팡이 1.0초, 화살 0.8초
- 마우스 방향 계산: `Camera.main.ScreenToWorldPoint` → 방향 벡터

**해골 지팡이 (Staff)**:

- `Physics2D.OverlapCircle` + 각도 필터 (전방 90도)
- 반경 1.8 유닛, 범위 내 모든 Enemy 레이어에 피해
- 넉백: 1.5 유닛 (적에게 `Rigidbody2D.AddForce`)
- 애니메이션 타이밍: 선딜 0.1초 → 활성 0.15초 → 후딜 0.05초

**영혼 화살 (SoulArrow)**:

- `Projectile` 프리팹 스폰 (ObjectPoolManager 사용)
- 속도: 12 유닛/초, 크기: 0.3×0.3, 사거리: 8 유닛
- 자동 조준 보정: 마우스 방향 ±15도 내 `Physics2D.OverlapCircle`로 가장 가까운 적 탐지 → 약한 유도
- 1회 적중 시 소멸 (관통 없음, 레벨업으로 관통 추가)

### 기존 코드 변경점

| 파일                   | 변경 내용                                    |
| ---------------------- | -------------------------------------------- |
| `PlayerController.cs`  | 좌클릭 입력 바인딩 (InputSystem Action 추가) |
| `ObjectPoolManager.cs` | Projectile 프리팹 풀 등록                    |

### 테스트 기준

- [ ] 좌클릭으로 공격 발동
- [ ] 지팡이: 전방 90도 부채꼴 범위 내 적에게 피해
- [ ] 지팡이: 넉백 1.5 유닛 적용
- [ ] 화살: 마우스 방향으로 투사체 발사
- [ ] 화살: 적 적중 시 피해 + 소멸
- [ ] 공격 쿨타임 작동
- [ ] 이동 중 공격 가능

---

## Step 1C. 경험치/레벨업 시스템

> 기획 문서: [progression-system.md](./progression-system.md), [enemy-wave-system.md](./enemy-wave-system.md)

### 구현 범위

1. `ExperienceSystem` — 경험치 관리, 레벨업 판정
2. `ExpOrb` — 경험치 구슬 오브젝트 (IPoolable)
3. `ExpCollector` — 경험치 수집 트리거
4. `LevelUpManager` — 선택지 풀 관리, 선택지 생성
5. `LevelUpUI` — 레벨업 시 3개 카드 표시 UI

### 코드 구조

```
Assets/Scripts/
├── Core/
│   ├── ExperienceSystem.cs       ← 신규: 레벨/XP 관리, 레벨업 판정
│   ├── ExpOrb.cs                 ← 신규: 경험치 구슬 (IPoolable, Suction)
│   └── ExpOrbSpawner.cs          ← 신규: 적 사망 시 구슬 스폰
├── Player/
│   └── ExpCollector.cs           ← 신규: 경험치 수집 트리거 (CorpseCollector 유사)
├── Skills/
│   ├── LevelUpManager.cs         ← 신규: 선택지 풀, 가중치 추출
│   ├── SkillOptionSO.cs          ← 신규: 개별 선택지 SO (이름, 효과, 가중치, 카테고리)
│   └── SkillCategory.cs          ← 신규: enum (NecromancerBuff, SummonBuff, LegionBuff)
├── UI/
│   └── LevelUpUI.cs              ← 신규: 3개 카드 UI
```

### 핵심 구현 사항

**ExperienceSystem (PlayerCombat에 추가하거나 별도 컴포넌트)**:

- `CurrentLevel`, `CurrentXP`, `XPToNextLevel`
- `AddExperience(int xp)`: XP 누적 → 레벨업 판정
- 필요 XP 테이블: `[10, 15, 22, 30, 40, ...]` (×1.3 배율)
- 레벨업 시 `GameEvents.RaiseLevelUp(int level)` 발행

**LevelUpManager**:

- SO 기반 선택지 풀 (에디터에서 ScriptableObject 에셋 생성)
- `GenerateChoices()`: 카테고리별 1개씩, 가중치 기반 랜덤 추출
- 선택지 상한 도달 시 풀에서 제거
- `ApplyChoice(SkillOptionSO option)`: 선택 효과 적용

**LevelUpUI**:

- `Time.timeScale = 0` 설정
- 3개 카드 표시 (이름, 효과 설명, 아이콘)
- 클릭 또는 1/2/3 키로 선택
- 선택 후 `Time.timeScale = 1` 복귀

### 기존 코드 변경점

| 파일            | 변경 내용                                                              |
| --------------- | ---------------------------------------------------------------------- |
| `GameEvents.cs` | `OnLevelUp(int level)`, `OnExperienceChanged(float ratio)` 이벤트 추가 |
| `EnemyBase.cs`  | 사망 시 ExpOrbSpawner 연동 (또는 GameEvents 통해)                      |

### 테스트 기준

- [ ] 적 사망 시 경험치 구슬 드롭
- [ ] 구슬 접근 시 자동 흡입 수집
- [ ] XP 누적 → 레벨업 판정
- [ ] 레벨업 시 게임 일시정지 + 3개 선택지 표시
- [ ] 선택 시 효과 적용 + 게임 재개
- [ ] 스탯 상한 도달 시 풀에서 제거

---

## Step 2A. Refine 합성 시스템

> 기획 문서: [corpse-economy.md](./corpse-economy.md)

### 구현 범위

1. `RefineSystem` — 합성 로직, 레시피 관리
2. `RefineRecipeSO` — 합성 레시피 SO
3. `RefineUI` — 합성 메뉴 UI

### 코드 구조

```
Assets/Scripts/
├── Player/
│   ├── RefineSystem.cs           ← 신규: 합성 로직 (Space 키)
│   └── SummonSystem.cs           ← 기존 유지
├── Skills/
│   └── RefineRecipeSO.cs         ← 신규: 합성 레시피 SO
├── UI/
│   └── RefineUI.cs               ← 신규: 합성 메뉴 UI
```

### 핵심 구현 사항

**RefineRecipeSO**:

```csharp
[CreateAssetMenu(menuName = "BoneToPeak/Refine Recipe")]
public class RefineRecipeSO : ScriptableObject
{
    public string RecipeName;
    public MinionStatsSO ResultMinion;       // 결과 미니언
    public MinionStatsSO[] RequiredMinions;   // 재료 미니언 SO 목록
    public int AdditionalCorpseCost;          // 추가 Corpse 비용
    public float CraftDuration;              // 합성 소요 시간
    public bool IsUnlocked;                  // 레시피 해금 여부 (T2는 기본 공개, T3는 조건부)
}
```

**RefineSystem**:

- Space 키 → 합성 메뉴 열기 (일시정지 아님, 실시간)
- 현재 소환된 미니언 목록에서 합성 가능한 조합 탐색
- 합성 시작 → 재료 미니언 정지 + 프로그레스 바 표시
- 합성 완료 → 재료 사라짐 + 결과 미니언 등장
- 합성 중 피격 → 합성 취소 (재료 보존, Corpse 반환)
- T3 레시피 해금: 재료 미니언 모두 동시 보유 이력 확인

### 기존 코드 변경점

| 파일                  | 변경 내용                                                               |
| --------------------- | ----------------------------------------------------------------------- |
| `GameEvents.cs`       | `OnRefineStarted`, `OnRefineCompleted`, `OnRefineCancelled` 이벤트 추가 |
| `PlayerCombat.cs`     | `OnDamageTaken` 이벤트 추가 → RefineSystem에서 피격 감지용              |
| `MinionBase.cs`       | `SetFrozen(bool)` 메서드 추가 — 합성 중 이동/공격 정지                  |
| `PlayerController.cs` | Space 키 입력 바인딩                                                    |

### 테스트 기준

- [ ] Space 키로 합성 메뉴 열기/닫기
- [ ] 합성 가능한 조합만 활성화 표시
- [ ] 합성 시작 시 재료 미니언 정지 + 프로그레스 바
- [ ] 합성 완료 시 결과 미니언 등장
- [ ] 합성 중 피격 시 취소 + 재료 보존
- [ ] Corpse 부족 시 합성 불가

---

## Step 2B. Explode 시체 폭발

> 기획 문서: [corpse-economy.md](./corpse-economy.md)

### 구현 범위

1. `ExplodeSystem` — 폭발 로직, 게이지 관리
2. 폭발 이펙트 및 피해 판정

### 코드 구조

```
Assets/Scripts/
├── Player/
│   └── ExplodeSystem.cs          ← 신규: E 키, 게이지, 폭발 피해 계산
```

### 핵심 구현 사항

**ExplodeSystem**:

- E 키 짧게: `ExplodeAll()` — 보유 Corpse 전량 소비
- E 키 길게: `StartCharging()` → 게이지 충전 → E 해제 시 `ExplodePartial(amount)`
- 게이지 충전 속도: 초당 보유 Corpse의 50%
- 쿨타임: 8초
- 피해 계산: `Physics2D.OverlapCircle` (폭발 반경) → 거리별 선형 보간 피해
- 연쇄 폭발: 폭발 반경 내 바닥 Corpse에 대해 2차 `OverlapCircle` (반경 2.0, 피해 15)
- 보스 내성: 피해 ×0.7

### 기존 코드 변경점

| 파일                  | 변경 내용                                                              |
| --------------------- | ---------------------------------------------------------------------- |
| `GameEvents.cs`       | `OnExplodeUsed(int corpseCount, float damage)` 이벤트 추가             |
| `PlayerCombat.cs`     | `ConsumeCorpse(int amount)` public 메서드 필요                         |
| `PlayerController.cs` | E 키 입력 바인딩 (Press/Release 구분)                                  |
| `Corpse.cs`           | `TriggerChainExplosion()` 메서드 추가 — 연쇄 폭발 시 피해 발생 후 소멸 |

### 테스트 기준

- [ ] E 키 짧게 눌러 전량 폭발
- [ ] E 키 길게 눌러 게이지 충전 → 부분 폭발
- [ ] 거리별 피해 감소 (100% → 70% → 40%)
- [ ] 바닥 Corpse 연쇄 폭발
- [ ] 쿨타임 8초 작동
- [ ] 보스 피해 70%
- [ ] 아군 미니언 피해 없음

---

## Step 3A. 엘리트 적

> 기획 문서: [enemy-wave-system.md](./enemy-wave-system.md)

### 구현 범위

1. 엘리트 3종 (기사, 마법사, 암살자) 개별 AI 클래스
2. `EnemySpawner` 확장 — 엘리트 스폰 확률

### 코드 구조

```
Assets/Scripts/
├── Enemies/
│   ├── EnemyBase.cs              ← 기존: 기본 AI (추적)
│   ├── EliteKnight.cs            ← 신규: EnemyBase 상속, 돌진 패턴
│   ├── EliteMage.cs              ← 신규: EnemyBase 상속, 범위 마법
│   ├── EliteAssassin.cs          ← 신규: EnemyBase 상속, 은신/우회
│   └── EnemySpawner.cs           ← 수정: 엘리트 스폰 확률 추가
```

### 핵심 구현 사항

- `EnemyBase`를 상속하여 각 엘리트 특수 능력 구현
- 엘리트 스케일링: 일반 스케일링 × 엘리트 배율 (중첩)
- 시각: 붉은색 외곽선 + 10% 크기 확대
- `EnemySpawner`에서 웨이브 5+ 부터 10% 확률로 엘리트 스폰

---

## Step 3B. 보스 시스템

> 기획 문서: [enemy-wave-system.md](./enemy-wave-system.md)

### 구현 범위

1. `BossBase` — 보스 공통 로직 (등장 연출, 페이즈 전환, 면역)
2. 보스 4종 개별 클래스

### 코드 구조

```
Assets/Scripts/
├── Enemies/
│   ├── BossBase.cs               ← 신규: EnemyBase 상속, 페이즈 관리, 등장 연출
│   ├── BossPaladin.cs            ← 신규: 2 페이즈
│   ├── BossDragon.cs             ← 신규: 3 페이즈
│   ├── BossArchmage.cs           ← 신규: 3 페이즈
│   ├── BossHero.cs               ← 신규: 4 페이즈
│   └── EnemySpawner.cs           ← 수정: 5/10/15/20 웨이브 보스 스폰
```

### 핵심 구현 사항

**BossBase**:

- 등장 연출 코루틴 (2초 이동 + 무적)
- `CurrentPhase`, `PhaseThresholds[]` — HP 비율 기반 페이즈 전환
- 페이즈 전환 시 0.5초 정지 + 시각 이펙트
- 넉백 면역, Explode 70%, 슬로우 50%
- 처치 시 Corpse 10개 + 보스 전용 드롭
- 체력바 이벤트: `OnBossHealthChanged(float ratio, string name)`

### 테스트 기준 (보스별)

- [ ] 등장 연출 (2초 이동, 이름 표시, 카메라 줌아웃)
- [ ] 각 페이즈 HP 구간 전환
- [ ] 페이즈별 패턴 정상 발동
- [ ] 보스 면역/저항 적용
- [ ] 처치 시 Corpse 10개 + 특수 드롭

---

## Step 4A. UI/HUD 통합

> 기획 문서: [player-necromancer.md](./player-necromancer.md)

### 구현 범위

1. HUD 통합 — HP 바, Corpse 카운터, 웨이브 타이머, 포메이션 표시, 미니언 카운터, Explode 쿨타임
2. 보스 HP 바 (보스 등장 시 표시)
3. 미니맵 (기본 구현)
4. 소환 휠 UI (우클릭 길게)

### 코드 구조

```
Assets/Scripts/
├── UI/
│   ├── HUDManager.cs             ← 신규: HUD 전체 관리
│   ├── HealthBarUI.cs            ← 신규: 좌상단 HP 바
│   ├── CorpseCounterUI.cs        ← 신규: 좌상단 Corpse 카운터
│   ├── WaveTimerUI.cs            ← 신규: 우상단 웨이브 & 타이머
│   ├── FormationIndicatorUI.cs   ← 신규: 좌하단 포메이션 표시
│   ├── MinionCounterUI.cs        ← 신규: 우하단 미니언 카운터
│   ├── ExplodeCooldownUI.cs      ← 신규: 우하단 Explode 쿨타임
│   ├── BossHealthBarUI.cs        ← 신규: 중앙상단 보스 HP 바
│   ├── SummonWheelUI.cs          ← 신규: 방사형 소환 메뉴
│   ├── MinimapUI.cs              ← 신규: 우상단 미니맵
│   └── LevelUpUI.cs              ← Step 1C에서 생성
```

### 핵심 구현 사항

- 모든 UI는 **GameEvents 구독** 방식으로 데이터 연동 (UI → 게임 로직 의존 없음)
- Canvas 기반 UI, `Screen Space - Overlay` 모드
- 각 UI 요소는 독립적 MonoBehaviour (HUDManager가 참조만 보유)

### GameEvents 연동 테이블

| UI 요소              | 구독 이벤트                              |
| -------------------- | ---------------------------------------- |
| HealthBarUI          | `OnPlayerHealthChanged(float ratio)`     |
| CorpseCounterUI      | `OnCorpseCountChanged(int count)`        |
| WaveTimerUI          | `OnWaveStarted(int)`, `OnWaveEnded(int)` |
| FormationIndicatorUI | `OnFormationChanged(FormationType)`      |
| MinionCounterUI      | `OnMinionCountChanged(int cur, int max)` |
| ExplodeCooldownUI    | `OnExplodeUsed(int, float)`              |
| BossHealthBarUI      | `OnBossHealthChanged(float, string)`     |
| LevelUpUI            | `OnLevelUp(int)`                         |

---

## 신규 GameEvents 목록

Phase 3에서 추가해야 할 GameEvents 이벤트 목록이다.

```csharp
// Phase 3 추가 이벤트
public static event Action<FormationType> OnFormationChanged;      // 포메이션 전환
public static event Action<int> OnLevelUp;                          // 레벨업
public static event Action<float> OnExperienceChanged;              // 경험치 비율 변화
public static event Action OnRefineStarted;                         // 합성 시작
public static event Action<MinionStatsSO> OnRefineCompleted;        // 합성 완료
public static event Action OnRefineCancelled;                       // 합성 취소
public static event Action<int, float> OnExplodeUsed;               // 폭발 사용 (Corpse 수, 데미지)
public static event Action<float, string> OnBossHealthChanged;      // 보스 HP 변화
public static event Action<string> OnBossSpawned;                   // 보스 등장
public static event Action<string> OnBossDefeated;                  // 보스 처치
```

---

## ScriptableObject 에셋 생성 가이드

Phase 3에서 Unity Editor에서 생성해야 할 SO 에셋 목록이다.

### 합성 레시피 SO (6개)

| 에셋명             | 결과          | 재료                   | 추가 Corpse |
| ------------------ | ------------- | ---------------------- | ----------- |
| Recipe_GraveKnight | 그래브 나이트 | 워리어 ×2              | 2           |
| Recipe_Wraith      | 레이스        | 아처 ×2                | 3           |
| Recipe_BoneGolem   | 본 골렘       | 좀비 ×3                | 3           |
| Recipe_DeathKnight | 데스 나이트   | 그래브 나이트 + 레이스 | 5           |
| Recipe_BoneDragon  | 본 드래곤     | 본 골렘 + 아처 ×3      | 8           |
| Recipe_Lich        | 리치          | 레이스 ×2              | 10          |

### 레벨업 선택지 SO (15개)

카테고리 A(5종) + B(5종) + C(5종), 각각 `SkillOptionSO` 에셋으로 생성.

### 보스 스탯 SO (4개)

| 에셋명        | HP   | 공격력 | 이동속도 | 페이즈 수 |
| ------------- | ---- | ------ | -------- | --------- |
| Boss_Paladin  | 500  | 20     | 5.0      | 2         |
| Boss_Dragon   | 1200 | 25     | 4.0      | 3         |
| Boss_Archmage | 800  | 30     | 3.0      | 3         |
| Boss_Hero     | 2000 | 35     | 5.0      | 4         |

### 엘리트 스탯 SO (3개)

기존 `EnemyStatsSO`와 동일 형식, 엘리트 플래그 추가.

---

## 물리 레이어 추가

Phase 3에서 필요한 추가 물리 레이어 설정 (Unity Editor).

| 레이어         | 번호 | 용도                               |
| -------------- | ---- | ---------------------------------- |
| Projectile     | 10   | 투사체 (영혼 화살, 보스 마법탄 등) |
| BossProjectile | 11   | 보스 전용 투사체                   |

### 충돌 매트릭스 추가

|                | Player | Enemy | Minion | Corpse | Projectile | BossProjectile |
| -------------- | ------ | ----- | ------ | ------ | ---------- | -------------- |
| Projectile     | X      | O     | X      | X      | X          | X              |
| BossProjectile | O      | X     | O      | X      | X          | X              |

> O = 충돌, X = 무시

---

## 개발 원칙

1. **기존 코드 최소 변경**: 가능한 한 새 클래스 추가로 해결, 기존 클래스는 이벤트/인터페이스로 연결
2. **ScriptableObject 활용**: 모든 밸런스 수치는 SO에서 관리 (코드에 매직 넘버 X)
3. **Object Pooling 필수**: 모든 동적 생성 오브젝트는 `ObjectPoolManager` 활용
4. **GameEvents 기반 통신**: 시스템 간 직접 참조 최소화, 이벤트 버스 활용
5. **네임스페이스 규칙**: `BoneToPeak.{모듈}` (예: `BoneToPeak.Skills`, `BoneToPeak.UI`)
