# Bone-To-Peak Project Setup Plan

> 작성일: 2026-02-25
> 수정일: 2026-02-25(Git LFS 옵션으로 변경)

---

## 1. 개발 환경 구성

| 항목     | 내용            |
| -------- | --------------- |
| Engine   | Unity 6.3 LTS   |
| IDE      | Rider / VS Code |
| VCS      | Git + Git LFS   |
| Platform | Steam (PC)      |

### TODO

- [ ] Unity Hub 설치
- [ ] Unity 6.3 LTS 설치
- [ ] Git LFS 설치 및 `.gitattributes` 설정
- [ ] `.gitignore` (Unity 템플릿) 추가

---

## 2. 프로젝트 구조 (예정)

```
bone-to-peak/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # GameManager, ObjectPool, EventSystem
│   │   ├── Player/         # Necromancer 컨트롤러
│   │   ├── Minions/        # 소환수 AI, 스폰, 포메이션
│   │   ├── Enemies/        # 적 AI, 웨이브 시스템
│   │   ├── Skills/         # ScriptableObject 기반 스킬
│   │   └── UI/             # HUD, 메뉴
│   ├── Prefabs/
│   ├── ScriptableObjects/
│   ├── Art/
│   ├── Audio/
│   └── Scenes/
├── Packages/
├── ProjectSettings/
├── docs/
└── README.md
```

---

## 3. 핵심 시스템 구현 순서

### Phase 1 — Foundation

- [ ] Unity 프로젝트 생성 (2D URP)
- [ ] Object Pooling 시스템
- [ ] 기본 Player 이동
- [ ] 카메라 Follow

### Phase 2 — Core Loop

- [ ] 적 스폰 / 웨이브 시스템
- [ ] 적 처치 시 Corpse 드롭
- [ ] 소환 시스템 (Raise)
- [ ] 미니언 기본 AI (Follow + Attack)

### Phase 3 — Corpse Economy

- [ ] Corpse 수집 및 자원 관리
- [ ] Refine (약한 미니언 → 강한 미니언 합성)
- [ ] Explode (시체 폭발 트랩)

### Phase 4 — Tactical Formations

- [ ] Defensive Phalanx
- [ ] Aggressive Charge
- [ ] Circular Guard
- [ ] Flocking Behavior (경량 군집 AI)

### Phase 5 — Progression & Polish

- [ ] 레벨업 / 스킬 선택 UI
- [ ] ScriptableObject 기반 스킬 모듈
- [ ] 사운드 / VFX
- [ ] 밸런싱 및 QA

---

## 4. 기술 메모

- **Object Pooling**: 수백 개의 미니언 + 수천 적 동시 처리 — Instantiate/Destroy 대신 풀링 필수
- **Flocking**: Boids 알고리즘 경량 변형 적용 (Separation, Alignment, Cohesion)
- **ScriptableObject**: 스킬/소환 조합을 데이터 드리븐으로 관리
