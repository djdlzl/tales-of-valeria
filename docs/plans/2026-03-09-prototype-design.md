# 테일즈오브발레리아 — 2단계 프로토타입 설계

> 작성일: 2026-03-09
> 목표: "클릭해서 이동하고, 적과 싸울 수 있으면 성공"
> 검증 기준: "이게 일렌시아처럼 느껴지는가?"

---

## 핵심 결정

| 항목 | 결정 | 이유 |
|------|------|------|
| 이동 방식 | NavMesh | 장애물 회피, 나중에 맵 확장 시 재사용 가능 |
| 전투 시작 | 적 클릭 → 자동 공격 | 일렌시아 그 자체 |
| 적 AI | 상태 머신 (3단계) | 간단하면서도 "살아있는 적" 느낌 |
| HP UI | 없음 (숫자만) | 프로토타입 범위 밖, YAGNI |

---

## 아키텍처

### 스크립트 구성

```
Assets/Scripts/
├── Character/
│   ├── PlayerController.cs   클릭 이동 + 타겟 지정
│   └── PlayerCombat.cs       자동 공격 루프, 플레이어 HP
└── Enemy/
    ├── EnemyAI.cs            상태 머신 (Idle / Chase / Attack)
    └── EnemyHealth.cs        적 HP + 사망 처리
```

### GameObject 구성

**Player**
- NavMeshAgent (이동)
- CapsuleCollider
- PlayerController.cs
- PlayerCombat.cs

**Enemy**
- NavMeshAgent (추적)
- CapsuleCollider (클릭 감지)
- EnemyAI.cs
- EnemyHealth.cs

---

## 시스템 흐름

### 이동
```
마우스 좌클릭
  └── Physics.Raycast (레이어: Ground)
        └── NavMeshAgent.SetDestination(hit.point)
```

### 전투
```
마우스 좌클릭
  └── Physics.Raycast (레이어: Enemy)
        └── PlayerCombat.SetTarget(enemy)
              └── 공격 범위(2m) 이내?
                    ├── YES → 자동 공격 (1.5초 쿨다운)
                    │         └── EnemyHealth.TakeDamage(10)
                    │               └── HP ≤ 0 → Destroy(enemy)
                    └── NO  → NavMesh로 적에게 이동 후 공격
```

### 적 AI 상태 머신
```
[Idle]
  └── 플레이어가 감지 범위(10m) 이내 → [Chase]

[Chase]
  └── NavMeshAgent로 플레이어 추적
  └── 공격 범위(2m) 이내 → [Attack]
  └── 플레이어가 감지 범위 밖 → [Idle]

[Attack]
  └── 1.5초마다 PlayerCombat.TakeDamage(5)
  └── 플레이어가 공격 범위 밖 → [Chase]
```

---

## 수치 (프로토타입 기본값)

| 속성 | 플레이어 | 적 |
|------|---------|-----|
| HP | 100 | 30 |
| 공격력 | 10 | 5 |
| 공격 쿨다운 | 1.5초 | 1.5초 |
| 공격 범위 | 2m | 2m |
| 이동 속도 | 5 | 3.5 |
| 감지 범위 | — | 10m |

---

## Unity 씬 설정 (수동 작업)

1. **NavMesh Bake**
   - Window → AI → Navigation → Bake
   - Plane이 Walkable로 등록되어야 함

2. **Layer 설정**
   - `Ground` 레이어: Plane에 할당
   - `Enemy` 레이어: Enemy GameObject에 할당

3. **씬 구성**
   - Plane (바닥, Ground 레이어)
   - Player Capsule (NavMeshAgent 포함)
   - Enemy Capsule x 2~3개 (NavMeshAgent 포함)
   - Main Camera (IsometricCamera.cs, Target = Player)

---

## 이번 단계 범위 밖 (다음 단계에서)

- HP 바 UI (3단계)
- 스킬 / 마법 (3단계)
- 애니메이션 (3단계)
- 사운드 (6단계)
- 멀티플레이 (6단계)
