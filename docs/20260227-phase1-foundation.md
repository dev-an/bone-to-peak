# Phase 1 — Foundation 구현 계획

> 작성일: 2026-02-27

---

## 목표

플레이어 캐릭터가 화면에서 움직이고, 카메라가 따라가며, 이후 대량 오브젝트 생성을 위한 풀링 시스템을 갖추는 것.

---

## 1. 기본 Player 이동

### 구현 내용

- Necromancer 게임 오브젝트 생성 (임시 스프라이트)
- Unity Input System으로 WASD/방향키 입력 처리
- Rigidbody2D 기반 이동 (물리 충돌 대응)
- 이동 속도 `[SerializeField]`로 Inspector 노출

### 스크립트

| 파일 | 네임스페이스 | 역할 |
| ---- | ------------ | ---- |
| `PlayerController.cs` | `BoneToPeak.Player` | 입력 수신 및 이동 처리 |
| `PlayerInputActions` | — | Input System 액션 맵 (자동 생성) |

### 완료 조건

- [ ] 씬에서 캐릭터가 WASD로 이동
- [ ] 이동 속도를 Inspector에서 조절 가능
- [ ] 대각선 이동 시 속도 정규화

---

## 2. 카메라 Follow

### 구현 내용

- Cinemachine 2D Virtual Camera 설정
- 플레이어를 Follow 타겟으로 지정
- Damping 값 조정으로 부드러운 추적

### 설정

| 항목 | 값 |
| ---- | -- |
| Body | Framing Transposer |
| Damping X/Y | 0.5 ~ 1.0 |
| Dead Zone | 0.1 |
| Lens Ortho Size | 게임 플레이에 맞춰 조정 |

### 완료 조건

- [ ] 카메라가 플레이어를 부드럽게 추적
- [ ] 플레이어가 화면 밖으로 나가지 않음

---

## 3. Object Pooling 시스템

### 구현 내용

- 범용 오브젝트 풀 (미니언, 적, 이펙트 등에 재사용)
- `IPoolable` 인터페이스로 풀 대상 오브젝트 규격화
- 풀 크기 자동 확장 (초기 크기 초과 시)

### 스크립트

| 파일 | 네임스페이스 | 역할 |
| ---- | ------------ | ---- |
| `ObjectPool.cs` | `BoneToPeak.Core` | 풀 생성/반환/확장 관리 |
| `ObjectPoolManager.cs` | `BoneToPeak.Core` | 여러 풀을 중앙에서 관리 |
| `IPoolable.cs` | `BoneToPeak.Core` | 풀 대상 오브젝트 인터페이스 |

### IPoolable 인터페이스

```csharp
namespace BoneToPeak.Core
{
    public interface IPoolable
    {
        void OnSpawn();   // 풀에서 꺼낼 때
        void OnDespawn(); // 풀로 반환할 때
    }
}
```

### 완료 조건

- [ ] 오브젝트를 풀에서 꺼내고 반환할 수 있음
- [ ] Instantiate/Destroy 없이 재사용 확인
- [ ] 풀 크기 초과 시 자동 확장

---

## 구현 순서

```
Player 이동 → 카메라 Follow → Object Pooling
```

Player 이동이 가장 먼저인 이유: 씬에서 동작하는 캐릭터가 있어야 카메라 테스트가 가능하고, 풀링은 Phase 2 적 스폰 직전에 있으면 된다.

---

## 기술 참고

- Unity Input System: `Packages/manifest.json`에 이미 포함
- Cinemachine: Package Manager에서 추가 필요
- Object Pooling: Unity 내장 `ObjectPool<T>` 또는 커스텀 구현 선택
