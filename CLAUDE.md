# Bone-To-Peak

다크 판타지 Bullet Heaven (Survivors-like) 게임. 네크로맨서가 적의 시체로 언데드 군단을 지휘한다.

## 기술 스택

| 항목     | 내용                   |
| -------- | ---------------------- |
| Engine   | Unity 6.3 LTS (2D URP) |
| Language | C# (.NET)              |
| IDE      | Rider / VS Code        |
| VCS      | Git + Git LFS          |
| Platform | Steam (PC)             |

## 디렉토리 구조

```
bone-to-peak/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/              # GameManager, ObjectPool, EventSystem
│   │   ├── Player/            # Necromancer 이동, 입력, 상태
│   │   ├── Minions/           # 소환수 AI, 스폰, 포메이션
│   │   ├── Enemies/           # 적 AI, 웨이브 시스템, 스포너
│   │   ├── Skills/            # ScriptableObject 기반 스킬
│   │   └── UI/                # HUD, 메뉴, 팝업
│   ├── Prefabs/               # 프리팹
│   ├── ScriptableObjects/     # SO 데이터 에셋
│   ├── Art/                   # 텍스처, 스프라이트
│   ├── Audio/                 # 사운드, 음악
│   ├── Scenes/                # 씬 파일
│   └── Settings/              # URP 렌더러 설정
├── Packages/                  # Unity 패키지 매니페스트
├── ProjectSettings/           # Unity 프로젝트 설정
├── docs/                      # 프로젝트 문서
│   ├── game-design/           # 게임 기획 문서
│   ├── commit-history/        # 개발 일지
│   └── coding-conventions.md  # C#/Unity 코딩 컨벤션
└── .gitattributes             # Git LFS 추적 설정
```

## C# 코딩 컨벤션

상세: `docs/coding-conventions.md`

- 네임스페이스: `BoneToPeak.{모듈}` (예: `BoneToPeak.Core`)
- 파일명 = 클래스명 (예: `PlayerController.cs`)
- 프라이빗 필드: `_camelCase`, 나머지: `PascalCase`
- ScriptableObject 클래스: `SO` 접미사 (예: `SkillDataSO`)
- `[SerializeField]`로 Inspector 노출, 퍼블릭 필드 지양
- 매직 넘버 대신 상수 또는 ScriptableObject 사용

## 참고 문서

| 문서                 | 경로                                     |
| -------------------- | ---------------------------------------- |
| 프로젝트 셋업 계획   | `docs/20260225-project-setup-plan.md`    |
| Unity 설치 가이드    | `docs/20260227-unity-setup-guide.md`     |
| 코딩 컨벤션          | `docs/coding-conventions.md`             |
| 게임 기획 총괄       | `docs/game-design/overview.md`           |
| 플레이어(네크로맨서) | `docs/game-design/player-necromancer.md` |
| 미니언 시스템        | `docs/game-design/minion-system.md`      |
| 시체 경제 시스템     | `docs/game-design/corpse-economy.md`     |
| 포메이션 시스템      | `docs/game-design/formation-system.md`   |
| 적 & 웨이브 시스템   | `docs/game-design/enemy-wave-system.md`  |
| 레벨 & 맵 디자인     | `docs/game-design/level-map-design.md`   |
| 성장 시스템          | `docs/game-design/progression-system.md` |

## 커밋 규칙

- Conventional Commits 형식: `<타입>: <제목>`
- 타입: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `revert`
- 한글로 작성
- 기능별로 묶어서 커밋 (한번에 모든 파일 커밋 금지)
- Claude 협력 문구 제외 (`Co-Authored-By` 등)

## 작업 내역 기록

- 작업 완료 시 `docs/commit-history/YYYYMMDD-{title}.md` 파일 생성/업데이트
- 커밋 타입별로 그룹화하여 정리

## 현재 진행 상황

Phase 1 — Foundation 구현 완료.
Phase 2 — Combat 기초 구현 완료.

### Phase 1 완료 항목

- Object Pooling 시스템 (IPoolable, ObjectPool, ObjectPoolManager)
- 플레이어 8방향 이동 (PlayerController + InputSystem)
- 카메라 Follow (Cinemachine 3.1.3)
- 씬 구성 및 테스트 통과

### Phase 2 완료 항목

- 게임 이벤트 시스템 (GameEvents — 정적 이벤트 버스)
- 체력/피격 시스템 (IDamageable, Health, DamageCalculator)
- 적/미니언 SO 데이터 (EnemyStatsSO, WaveConfigSO, MinionStatsSO)
- 플레이어 전투 (PlayerCombat — 시체 인벤토리, 소환 슬롯, 무적 시간)
- 기본 적 AI (EnemyBase — 플레이어 추적, 미니언 접촉 데미지)
- 적 스폰 시스템 (EnemySpawner — 웨이브 기반, 카메라 가장자리 스폰)
- 시체 드롭/수집 (Corpse, CorpseSpawner, CorpseCollector)
- 미니언 AI (MinionBase — Follow/Attack 상태, 적 접촉 데미지)
- 소환 시스템 (SummonSystem — 우클릭 소환)

### Unity Editor 수동 설정 (미완료)

> 상세 가이드: `docs/unity-editor-setup-phase2.md`

- 물리 레이어: Player(6), Enemy(7), Minion(8), Corpse(9) + 충돌 매트릭스
- SO 에셋 생성: 적 4종, 웨이브 4개, 미니언 3종 (AttackInterval = 공격 간격 초)
- 프리팹 생성: 적 4종, 미니언 3종, Corpse (임시 색상 스프라이트)
- 씬 구성: CorpseSpawner, EnemySpawner 매니저 + CorpseDetector 자식 오브젝트

## TODO (Phase 3)

1. 포메이션 시스템 (Guard 상태, 3가지 포메이션)
2. 플레이어 기본 공격 (지팡이/소울 애로우)
3. Refine 합성 시스템
4. Explode (시체 폭발)
5. 보스/엘리트 적
6. UI/HUD
