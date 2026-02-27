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

완료 항목:
- Object Pooling 시스템 (IPoolable, ObjectPool, ObjectPoolManager)
- 플레이어 8방향 이동 (PlayerController + InputSystem)
- 카메라 Follow (Cinemachine 3.1.3)
- 씬 구성 및 테스트 통과

## TODO (Phase 2 — Combat 기초)

1. 적 스폰 시스템 (EnemySpawner, 웨이브 기반)
2. 기본 적 AI (플레이어 방향 이동)
3. 체력/피격 시스템 (IDamageable)
4. 시체 드롭 시스템 (Corpse)
5. 기본 미니언 소환 (Raise)
