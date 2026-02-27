# 2026-02-27: Phase 1 — Foundation 구현

## feat

### Object Pooling 시스템 (Core)
- `IPoolable.cs`: 풀 스폰/반환 시 콜백 인터페이스 (`OnSpawnFromPool`, `OnReturnToPool`)
- `ObjectPool.cs`: `UnityEngine.Pool.ObjectPool<T>` 래핑, `Get()`, `Get(pos, rot)`, `Release()` 메서드, Prewarm 지원
- `ObjectPoolManager.cs`: 싱글톤 MonoBehaviour, 프리팹별 `Dictionary<GameObject, ObjectPool>` 관리, `Spawn`/`Despawn` API

### 플레이어 8방향 이동 (Player)
- `PlayerController.cs`: `InputActionAsset` 기반 Move 입력 처리
- 대각선 이동 정규화 (`sqrMagnitude > 1f`)
- Unity 6 API `Rigidbody2D.linearVelocity` 사용
- `OnEnable`/`OnDisable`에서 InputAction Enable/Disable

## chore

### Cinemachine 패키지 설치
- `Packages/manifest.json`에 `com.unity.cinemachine: 3.1.3` 추가

## Unity 에디터 수동 작업 (TODO)

- [ ] Player 오브젝트 생성: Square 스프라이트 + Rigidbody2D(Gravity 0, Freeze Rotation Z) + CircleCollider2D(0.4) + PlayerController
- [ ] PlayerController에 InputSystem_Actions 에셋 할당
- [ ] ObjectPoolManager 빈 GameObject 생성 + 스크립트 추가
- [ ] CinemachineCamera 추가 → Follow = Player, Damping 1/1, Ortho Size 7~8
