# Phase 2 — Unity Editor 수동 설정 가이드

> Phase 2 스크립트가 정상 동작하려면 아래 항목을 Unity Editor에서 직접 설정해야 합니다.

---

## 1. 물리 레이어 설정

**경로**: `Edit > Project Settings > Tags and Layers > Layers`

| 레이어 번호 | 이름     | 용도                      |
| ----------- | -------- | ------------------------- |
| 6           | Player   | 네크로맨서                |
| 7           | Enemy    | 모든 적 (일반/엘리트/보스) |
| 8           | Minion   | 소환된 미니언             |
| 9           | Corpse   | 바닥의 시체 오브젝트      |

---

## 2. 충돌 매트릭스 설정

**경로**: `Edit > Project Settings > Physics 2D > Layer Collision Matrix`

모든 레이어 간 충돌을 **비활성화**한 후, 아래 조합만 **활성화**합니다.

| 레이어 A | 레이어 B | 활성화 | 용도                                    |
| -------- | -------- | ------ | --------------------------------------- |
| Player   | Enemy    | O      | 적 접촉 시 플레이어 데미지 (OnTriggerStay2D) |
| Minion   | Enemy    | O      | 미니언의 적 탐색 (OverlapCircleNonAlloc) |
| Player   | Corpse   | O      | 시체 수집 트리거 (CorpseCollector)       |

**비활성화해야 하는 조합** (명시적):

| 레이어 A | 레이어 B | 비활성화 | 이유                             |
| -------- | -------- | -------- | -------------------------------- |
| Player   | Minion   | X        | 플레이어와 미니언은 통과         |
| Minion   | Minion   | X        | 미니언끼리 통과 (Flocking으로 분리) |
| Enemy    | Enemy    | X        | 적끼리 겹침 허용                 |
| Enemy    | Corpse   | X        | 적은 시체와 상호작용 없음        |
| Minion   | Corpse   | X        | 미니언은 시체를 수집하지 않음    |
| Corpse   | Corpse   | X        | 시체끼리 물리 충돌 없음          |

---

## 3. ScriptableObject 에셋 생성

**생성 방법**: `Assets/ScriptableObjects/` 폴더에서 우클릭 > `Create > BoneToPeak > ...`

### 3-1. 적 스탯 (EnemyStats) — 4종

`Create > BoneToPeak > Enemy Stats`

| 에셋 이름        | EnemyName  | MaxHealth | Attack | MoveSpeed | AttackRange | CorpseDropCount |
| ---------------- | ---------- | --------- | ------ | --------- | ----------- | --------------- |
| `EnemyStats_Swarm`     | 스웜       | 15        | 5      | 5.0       | 0.8         | 1               |
| `EnemyStats_Halberdier` | 할버디어   | 40        | 12     | 3.5       | 1.5         | 1               |
| `EnemyStats_Archer`    | 궁수       | 25        | 8      | 3.0       | 6.0         | 1               |
| `EnemyStats_Shielder`  | 방패병     | 80        | 6      | 2.5       | 0.8         | 2               |

### 3-2. 웨이브 설정 (WaveConfig) — 4개

`Create > BoneToPeak > Wave Config`

**Wave1_Tutorial**

| 필드           | 값    |
| -------------- | ----- |
| WaveNumber     | 1     |
| Duration       | 60    |
| SpawnInterval  | 2.0   |
| HpMultiplier   | 1.0   |
| AtkMultiplier  | 1.0   |
| SpdMultiplier  | 1.0   |
| RestDuration   | 10    |
| EnemyEntries   | Swarm 프리팹 (Weight: 1.0) |

**Wave2_Halberdier**

| 필드           | 값    |
| -------------- | ----- |
| WaveNumber     | 2     |
| Duration       | 60    |
| SpawnInterval  | 1.8   |
| HpMultiplier   | 1.0   |
| AtkMultiplier  | 1.0   |
| SpdMultiplier  | 1.0   |
| RestDuration   | 10    |
| EnemyEntries   | Swarm (Weight: 0.7), Halberdier (Weight: 0.3) |

**Wave3_Archer**

| 필드           | 값    |
| -------------- | ----- |
| WaveNumber     | 3     |
| Duration       | 60    |
| SpawnInterval  | 1.5   |
| HpMultiplier   | 1.0   |
| AtkMultiplier  | 1.0   |
| SpdMultiplier  | 1.0   |
| RestDuration   | 10    |
| EnemyEntries   | Swarm (Weight: 0.5), Halberdier (Weight: 0.25), Archer (Weight: 0.25) |

**Wave4_Mixed**

| 필드           | 값    |
| -------------- | ----- |
| WaveNumber     | 4     |
| Duration       | 60    |
| SpawnInterval  | 1.3   |
| HpMultiplier   | 1.0   |
| AtkMultiplier  | 1.0   |
| SpdMultiplier  | 1.0   |
| RestDuration   | 10    |
| EnemyEntries   | Swarm (0.4), Halberdier (0.2), Archer (0.2), Shielder (0.2) |

### 3-3. 미니언 스탯 (MinionStats) — 3종

`Create > BoneToPeak > Minion Stats`

| 에셋 이름              | MinionName      | Tier | MaxHealth | Attack | AttackInterval | MoveSpeed | AttackRange | CorpseCost | IsRanged |
| ---------------------- | --------------- | ---- | --------- | ------ | -------------- | --------- | ----------- | ---------- | -------- |
| `MinionStats_Warrior`  | 스켈레톤 워리어 | T1   | 30        | 12     | 1.0            | 4.5       | 1.5         | 1          | false    |
| `MinionStats_Archer`   | 스켈레톤 아처   | T1   | 20        | 8      | 1.2            | 4.0       | 7.0         | 2          | true     |
| `MinionStats_Zombie`   | 좀비            | T1   | 60        | 6      | 1.5            | 3.0       | 1.2         | 1          | false    |

> **주의**: `AttackInterval` 값은 **공격 간격(초)**입니다. 높을수록 느린 공격입니다.

---

## 4. 프리팹 생성

**경로**: `Assets/Prefabs/` 폴더

### 4-1. 적 프리팹 — 4종

각 적 프리팹에 공통으로 필요한 컴포넌트:

| 컴포넌트         | 설정                                                    |
| ---------------- | ------------------------------------------------------- |
| SpriteRenderer   | 임시 스프라이트 (색상으로 구분)                          |
| Rigidbody2D      | Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation Z |
| CircleCollider2D | Is Trigger: **true**                                    |
| EnemyBase        | Stats: 해당 EnemyStatsSO 에셋 할당                     |

| 프리팹 이름      | 스프라이트 색상 | Collider Radius |
| ---------------- | --------------- | --------------- |
| `Enemy_Swarm`     | 빨간색 (FF0000) | 0.3             |
| `Enemy_Halberdier`| 주황색 (FF8800) | 0.4             |
| `Enemy_Archer`    | 보라색 (8800FF) | 0.3             |
| `Enemy_Shielder`  | 회색 (888888)   | 0.5             |

**Layer**: 모든 적 프리팹의 Layer를 **Enemy (7)**로 설정

### 4-2. 미니언 프리팹 — 3종 (Phase 2에서는 1종만 필수)

각 미니언 프리팹에 공통으로 필요한 컴포넌트:

| 컴포넌트         | 설정                                                    |
| ---------------- | ------------------------------------------------------- |
| SpriteRenderer   | 임시 스프라이트 (색상으로 구분)                          |
| Rigidbody2D      | Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation Z |
| CircleCollider2D | Is Trigger: **true**                                    |
| MinionBase       | Stats: 해당 MinionStatsSO 에셋 할당                    |

| 프리팹 이름        | 스프라이트 색상   | Collider Radius |
| ------------------ | ----------------- | --------------- |
| `Minion_Warrior`    | 연두색 (88FF00)   | 0.3             |
| `Minion_Archer`     | 하늘색 (00CCFF)   | 0.3             |
| `Minion_Zombie`     | 갈색 (886600)     | 0.4             |

**Layer**: 모든 미니언 프리팹의 Layer를 **Minion (8)**으로 설정

### 4-3. 시체 프리팹 — 1종

| 컴포넌트         | 설정                                                    |
| ---------------- | ------------------------------------------------------- |
| SpriteRenderer   | 임시 스프라이트, 녹색 (00FF66), 크기 0.5x0.5            |
| CircleCollider2D | Is Trigger: **true**, Radius: 0.25                      |
| Corpse (스크립트)| 기본값 유지 (LifeTime: 5, FlashStartTime: 3.5)          |

**Layer**: **Corpse (9)**로 설정
**Rigidbody2D**: **불필요** (Corpse는 물리 이동 없이 코드로 position 직접 변경)

> **참고**: Collider2D가 트리거로 동작하려면 충돌 대상(Player) 중 하나에 Rigidbody2D가 있어야 합니다. Player에는 이미 Rigidbody2D가 있으므로 정상 동작합니다.

---

## 5. 플레이어 오브젝트 설정

**GameScene**에서 Player 오브젝트 구성:

### 5-1. Player (루트 오브젝트)

| 컴포넌트         | 설정                                                    |
| ---------------- | ------------------------------------------------------- |
| SpriteRenderer   | 임시 스프라이트, 흰색 또는 파란색                        |
| Rigidbody2D      | Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation Z |
| CircleCollider2D | Is Trigger: **true**, Radius: 0.4                       |
| PlayerController | InputActions: `InputSystem_Actions` 에셋 할당           |
| PlayerCombat     | 기본값 유지                                             |
| SummonSystem     | InputActions: `InputSystem_Actions`, DefaultMinionStats: `MinionStats_Warrior`, DefaultMinionPrefab: `Minion_Warrior` |

**Layer**: **Player (6)**

### 5-2. CorpseDetector (Player의 자식 오브젝트)

Player 오브젝트 하위에 빈 GameObject 생성 후:

| 컴포넌트         | 설정                                                    |
| ---------------- | ------------------------------------------------------- |
| CircleCollider2D | Is Trigger: **true**, Radius: **1.5** (수집 범위)       |
| CorpseCollector  | 기본값 유지 (SuctionSpeed: 10, CollectDistance: 0.3)    |

**Layer**: **Player (6)** (부모와 동일)

---

## 6. 씬 매니저 오브젝트 설정

**GameScene**에 빈 GameObject로 매니저들을 배치합니다.

### 6-1. ObjectPoolManager

| 컴포넌트          | 설정                                   |
| ----------------- | -------------------------------------- |
| ObjectPoolManager | DefaultInitialSize: 10, DefaultMaxSize: 100 |

> 이미 존재하면 중복 생성하지 마세요. `DontDestroyOnLoad`로 씬 전환에도 유지됩니다.

### 6-2. CorpseSpawner

| 컴포넌트      | 설정                                         |
| ------------- | -------------------------------------------- |
| CorpseSpawner | CorpsePrefab: `Corpse` 프리팹, ScatterRadius: 0.5 |

### 6-3. EnemySpawner

| 컴포넌트     | 설정                                                    |
| ------------ | ------------------------------------------------------- |
| EnemySpawner | WaveConfigs: [Wave1, Wave2, Wave3, Wave4] 순서대로 할당, SpawnOffsetFromCamera: 1.5 |

---

## 7. 카메라 설정

- Main Camera: **Orthographic**, Size는 게임 스케일에 맞게 조정 (권장: 5~7)
- Cinemachine Virtual Camera가 Player를 Follow하도록 설정 (Phase 1에서 완료)

---

## 8. 설정 체크리스트

완료 후 아래 항목을 확인합니다.

- [ ] 레이어 4종 설정 (Player/Enemy/Minion/Corpse)
- [ ] 충돌 매트릭스 설정 (Player-Enemy, Minion-Enemy, Player-Corpse만 활성화)
- [ ] EnemyStatsSO 에셋 4종 생성 및 값 입력
- [ ] WaveConfigSO 에셋 4종 생성 및 적 프리팹 연결
- [ ] MinionStatsSO 에셋 3종 생성 및 값 입력 (Phase 2에서는 Warrior 1종 필수)
- [ ] 적 프리팹 4종 생성 (SpriteRenderer + Rigidbody2D + CircleCollider2D + EnemyBase)
- [ ] 미니언 프리팹 생성 (Phase 2에서는 Warrior 1종 필수)
- [ ] Corpse 프리팹 생성 (SpriteRenderer + CircleCollider2D + Corpse)
- [ ] 모든 프리팹에 올바른 Layer 할당
- [ ] Player 오브젝트 컴포넌트 구성 (PlayerController + PlayerCombat + SummonSystem)
- [ ] CorpseDetector 자식 오브젝트 생성 (CircleCollider2D + CorpseCollector)
- [ ] ObjectPoolManager 씬에 배치
- [ ] CorpseSpawner 씬에 배치 + Corpse 프리팹 연결
- [ ] EnemySpawner 씬에 배치 + WaveConfig 4종 연결
- [ ] SummonSystem에 InputActions + MinionStats + MinionPrefab 할당

---

## 9. 동작 확인 방법

Play 모드 진입 후 예상 동작:

1. **이동**: WASD/방향키로 8방향 이동
2. **적 스폰**: 카메라 가장자리에서 적이 출현, 플레이어를 향해 이동
3. **접촉 데미지**: 적과 접촉 시 플레이어 깜빡임 + HP 감소 (콘솔 로그)
4. **적 처치**: 미니언이 적을 공격하여 처치 (콘솔에 사망 로그)
5. **시체 드롭**: 적 사망 위치에 녹색 시체 스폰
6. **시체 수집**: 플레이어가 시체 근처 접근 시 흡입 → 수집 (콘솔에 시체 수집 로그)
7. **소환**: 우클릭으로 미니언 소환 (시체 1개 소비, 콘솔 로그)
8. **미니언 AI**: Follow → 적 감지 시 Attack → 적 소멸 시 Follow 복귀
9. **시체 소멸**: 5초 후 깜빡이며 소멸 (3.5초부터 깜빡임 시작)
10. **웨이브 진행**: Wave 1~4 순차 진행, 웨이브 간 10초 휴식
