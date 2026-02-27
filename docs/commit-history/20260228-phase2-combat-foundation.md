# Phase 2 — Combat Foundation 구현

## 날짜: 2026-02-28

## 개요

Phase 2 전투 기초 시스템 구현 완료. 핵심 전투 루프를 구축:
**적 스폰 → 미니언이 처치 → 시체 드롭 → 수집 → 소환**

---

## feat

### 게임 이벤트 시스템 (GameEvents)
- `Assets/Scripts/Core/GameEvents.cs` — 정적 이벤트 버스
- `Assets/Scripts/Core/EnemyDeathEventArgs.cs` — struct 기반 이벤트 인자
- 이벤트: EnemyDeath, PlayerHealthChanged, PlayerDeath, CorpseCountChanged, MinionCountChanged, WaveStarted/Ended

### 체력/피격 시스템 (IDamageable, Health)
- `Assets/Scripts/Core/IDamageable.cs` — 인터페이스
- `Assets/Scripts/Core/Health.cs` — 순수 C# 클래스 (컴포지션 패턴)
- `Assets/Scripts/Core/DamageCalculator.cs` — max(1, ATK - DEF)

### 적/미니언 SO 데이터 정의
- `Assets/Scripts/Enemies/EnemyStatsSO.cs` — 적 스탯 데이터
- `Assets/Scripts/Enemies/WaveConfigSO.cs` — 웨이브 구성 + WaveEnemyEntry
- `Assets/Scripts/Minions/MinionStatsSO.cs` — 미니언 스탯 데이터
- `Assets/Scripts/Minions/MinionTier.cs` — enum (T1, T2, T3)

### 플레이어 전투 시스템 (PlayerCombat)
- `Assets/Scripts/Player/PlayerCombat.cs` — IDamageable 구현
- 시체 인벤토리 (10개 용량), 소환 슬롯 (3개), 무적 시간 (0.5초)
- 접촉 데미지 (OnTriggerStay2D), 알파 깜빡임 효과
- `PlayerController.cs`에 `static Instance` 프로퍼티 추가

### 기본 적 AI (EnemyBase)
- `Assets/Scripts/Enemies/EnemyBase.cs` — IDamageable + IPoolable
- 웨이브 배율 적용 (HP/ATK/SPD), 플레이어 추적 이동
- 사망 시 GameEvents 이벤트 발행 → 풀 반환

### 적 스폰 시스템 (EnemySpawner)
- `Assets/Scripts/Enemies/EnemySpawner.cs`
- WaveConfigSO 배열 기반 순차 웨이브 진행
- 카메라 가장자리 + 1.5 유닛 오프셋, 가중치 기반 적 선택

### 시체 드롭 및 수집 시스템 (Corpse)
- `Assets/Scripts/Core/Corpse.cs` — IPoolable, 5초 수명, 흡인 이동
- `Assets/Scripts/Core/CorpseSpawner.cs` — OnEnemyDeath 구독, scatter offset
- `Assets/Scripts/Player/CorpseCollector.cs` — CircleCollider2D trigger 기반 수집

### 기본 미니언 AI (MinionBase)
- `Assets/Scripts/Minions/MinionBase.cs` — IDamageable + IPoolable
- `Assets/Scripts/Minions/MinionState.cs` — enum (Follow, Attack)
- Follow: 플레이어 주변 원형 분포 유지
- Attack: OverlapCircleNonAlloc으로 적 탐색 → 추적 → 공격

### 소환 시스템 (SummonSystem)
- `Assets/Scripts/Player/SummonSystem.cs` — 우클릭 기본 미니언 소환
- `Assets/InputSystem_Actions.inputactions` — Summon 액션 추가

---

## docs

### 기획 문서 업데이트
- `docs/game-design/level-map-design.md` — 레벨 & 맵 디자인 신규 작성
- `docs/game-design/minion-system.md` — 슬롯/공격범위 수치 구체화
- `docs/game-design/overview.md` — 문서 링크 추가

---

## 신규 파일 (18개)

| 네임스페이스 | 파일 |
|---|---|
| BoneToPeak.Core | GameEvents.cs, EnemyDeathEventArgs.cs, IDamageable.cs, Health.cs, DamageCalculator.cs, Corpse.cs, CorpseSpawner.cs |
| BoneToPeak.Enemies | EnemyStatsSO.cs, WaveConfigSO.cs, EnemyBase.cs, EnemySpawner.cs |
| BoneToPeak.Minions | MinionStatsSO.cs, MinionTier.cs, MinionState.cs, MinionBase.cs |
| BoneToPeak.Player | PlayerCombat.cs, SummonSystem.cs, CorpseCollector.cs |

## 수정 파일 (1개)

| 파일 | 변경 |
|---|---|
| PlayerController.cs | static Instance 추가 |

## Unity Editor에서 수동 설정 필요

- [ ] 물리 레이어 설정: Player(6), Enemy(7), Minion(8), Corpse(9)
- [ ] 충돌 매트릭스: Enemy↔Player, Enemy↔Minion, Corpse↔Player만 활성화
- [ ] SO 에셋 생성 (CreateAssetMenu): 적 4종, 웨이브 4개, 미니언 3종
- [ ] 프리팹 생성: 적 4종, 미니언 3종, Corpse (임시 컬러 스프라이트)
- [ ] 씬 구성: CorpseSpawner/EnemySpawner 매니저, CorpseDetector 자식 오브젝트
