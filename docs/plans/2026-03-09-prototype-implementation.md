# 2단계 프로토타입 구현 계획

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 클릭 이동 + 적 클릭 자동공격 + 상태머신 적 AI — 일렌시아 스타일 전투 프로토타입 완성

**Architecture:** C# MonoBehaviour 4개 스크립트(EnemyHealth → PlayerController → PlayerCombat → EnemyAI) 순으로 작성. 의존성 방향: EnemyHealth ← PlayerCombat ← PlayerController, PlayerCombat ← EnemyAI. Unity NavMesh로 이동, Physics.Raycast로 클릭 감지.

**Tech Stack:** Unity 6 LTS, C# MonoBehaviour, NavMeshAgent, Physics.Raycast, IEnumerator Coroutine

---

## 사전 지식 (처음이면 꼭 읽을 것)

### Unity 레이어(Layer) 마스크
```csharp
// 특정 레이어에만 Raycast 충돌
int mask = LayerMask.GetMask("Ground");   // "Ground" 레이어만
Physics.Raycast(ray, out hit, 100f, mask); // 그 레이어만 감지
```

### NavMeshAgent
```csharp
NavMeshAgent agent = GetComponent<NavMeshAgent>();
agent.SetDestination(targetPosition); // 목표 위치 지정 → 자동 이동
agent.speed = 5f;                     // 이동 속도
```

### Coroutine (반복 공격에 사용)
```csharp
StartCoroutine(AttackLoop());

IEnumerator AttackLoop()
{
    while (조건)
    {
        // 공격 로직
        yield return new WaitForSeconds(1.5f); // 1.5초 대기
    }
}
```

---

## Task 1: EnemyHealth.cs — 적 HP + 사망 처리

**Files:**
- Create: `Assets/Scripts/Enemy/EnemyHealth.cs`

### Step 1: 스크립트 생성

```csharp
using UnityEngine;

/// <summary>
/// 적의 HP 관리 및 사망 처리
/// PlayerCombat.TakeDamage()가 이 스크립트를 호출함
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("적 수치")]
    public int maxHp = 30;

    private int _currentHp;

    void Start()
    {
        _currentHp = maxHp;
    }

    /// <summary>PlayerCombat에서 호출: 데미지 적용 후 HP 0이면 사망</summary>
    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        Debug.Log($"{gameObject.name} HP: {_currentHp}/{maxHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        Destroy(gameObject);
    }
}
```

### Step 2: Unity Editor에서 Enemy에 컴포넌트 추가

1. Hierarchy → Enemy Capsule 선택
2. Inspector → Add Component → `EnemyHealth` 입력 → 추가
3. Max Hp 필드가 30으로 표시되면 정상

### Step 3: Play 모드로 기본 검증

Play 버튼 → Console에 에러 없으면 OK

### Step 4: 커밋

```bash
cd /Users/harvey/workspace
GIT_TRACE=1 git add "game/Tales of Valeria/Assets/Scripts/Enemy/EnemyHealth.cs"
GIT_TRACE=1 git commit -m "feat: EnemyHealth.cs — 적 HP + 사망 처리"
```

> **주의:** git 커밋 시 `GIT_TRACE=1`을 앞에 붙여야 index.lock 문제를 피할 수 있음

---

## Task 2: PlayerController.cs — 클릭 이동 + 타겟 지정

**Files:**
- Create: `Assets/Scripts/Character/PlayerController.cs`

**전제 조건:**
- `Ground` 레이어가 Plane GameObject에 할당되어 있어야 함
- `Enemy` 레이어가 Enemy GameObject에 할당되어 있어야 함
- Player GameObject에 `NavMeshAgent` 컴포넌트가 추가되어 있어야 함
- NavMesh가 Bake되어 있어야 함 (Task 5 참고)

> **Layer 설정 (먼저 해야 함):**
> 1. Edit → Project Settings → Tags and Layers
> 2. User Layer 6: `Ground`, User Layer 7: `Enemy` 입력 (숫자는 비어있는 아무 슬롯)
> 3. Hierarchy → Plane 선택 → Inspector 상단 Layer 드롭다운 → `Ground` 선택
> 4. Hierarchy → Enemy Capsule 선택 → Inspector 상단 Layer 드롭다운 → `Enemy` 선택

### Step 1: 스크립트 생성

```csharp
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 마우스 좌클릭 처리:
///   - 바닥 클릭 → NavMesh 이동
///   - 적 클릭 → PlayerCombat.SetTarget() 호출
/// </summary>
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private PlayerCombat _combat;

    private int _groundMask;
    private int _enemyMask;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<PlayerCombat>();

        // 레이어 마스크 캐싱 (매 프레임 GetMask 호출 방지)
        _groundMask = LayerMask.GetMask("Ground");
        _enemyMask = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 1순위: 적 클릭 확인
        if (Physics.Raycast(ray, out RaycastHit enemyHit, 100f, _enemyMask))
        {
            EnemyHealth enemy = enemyHit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                _combat.SetTarget(enemy);
                return; // 적 클릭이면 이동 안 함
            }
        }

        // 2순위: 바닥 클릭 → 이동
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundMask))
        {
            _combat.ClearTarget();
            _agent.SetDestination(groundHit.point);
        }
    }
}
```

### Step 2: Player GameObject 설정

1. Hierarchy → Player Capsule 선택
2. Inspector → Add Component:
   - `NavMeshAgent` 추가
   - `PlayerController` 추가
   - (PlayerCombat은 Task 3에서 추가)
3. NavMeshAgent 설정:
   - Speed: `5`
   - Stopping Distance: `1.8` (공격 범위 2m보다 약간 작게)

### Step 3: Play 모드 검증

Play 버튼 → 바닥 클릭 → 캡슐이 클릭한 방향으로 이동하면 성공

> **NavMesh 미완료 시:** 이동이 안 될 수 있음. Task 5의 NavMesh Bake를 먼저 해도 됨.

### Step 4: 커밋

```bash
cd /Users/harvey/workspace
GIT_TRACE=1 git add "game/Tales of Valeria/Assets/Scripts/Character/PlayerController.cs"
GIT_TRACE=1 git commit -m "feat: PlayerController.cs — 클릭 이동 + 타겟 지정"
```

---

## Task 3: PlayerCombat.cs — 자동 공격 루프 + 플레이어 HP

**Files:**
- Create: `Assets/Scripts/Character/PlayerCombat.cs`

**의존성:** EnemyHealth.cs (Task 1 완료 후)

### Step 1: 스크립트 생성

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 자동 공격 시스템:
///   - SetTarget(enemy): 타겟 지정 → 범위 내면 공격, 범위 밖이면 접근
///   - ClearTarget(): 타겟 해제 (바닥 클릭 시)
///   - TakeDamage(dmg): EnemyAI에서 호출
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("전투 수치")]
    public int maxHp = 100;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f;

    private int _currentHp;
    private EnemyHealth _target;
    private NavMeshAgent _agent;
    private bool _isAttacking;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _currentHp = maxHp;
    }

    void Update()
    {
        if (_target == null)
        {
            // 타겟 소멸 시 공격 중단
            _isAttacking = false;
            return;
        }

        float dist = Vector3.Distance(transform.position, _target.transform.position);

        if (dist <= attackRange)
        {
            // 공격 범위 내: 제자리 정지
            _agent.SetDestination(transform.position);

            if (!_isAttacking)
            {
                StartCoroutine(AttackLoop());
            }
        }
        else
        {
            // 범위 밖: 타겟 추적
            _agent.SetDestination(_target.transform.position);
        }
    }

    /// <summary>PlayerController에서 적 클릭 시 호출</summary>
    public void SetTarget(EnemyHealth enemy)
    {
        _target = enemy;
        _isAttacking = false; // 코루틴 재시작을 위해 초기화
    }

    /// <summary>PlayerController에서 바닥 클릭 시 호출</summary>
    public void ClearTarget()
    {
        _target = null;
        _isAttacking = false;
    }

    private IEnumerator AttackLoop()
    {
        _isAttacking = true;

        while (_target != null)
        {
            float dist = Vector3.Distance(transform.position, _target.transform.position);
            if (dist > attackRange)
            {
                break; // 범위 벗어나면 Update()가 다시 접근 처리
            }

            _target.TakeDamage(attackDamage);
            Debug.Log($"플레이어 공격! (데미지: {attackDamage})");

            yield return new WaitForSeconds(attackCooldown);
        }

        _isAttacking = false;
    }

    /// <summary>EnemyAI에서 공격 시 호출</summary>
    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        Debug.Log($"플레이어 HP: {_currentHp}/{maxHp}");

        if (_currentHp <= 0)
        {
            Debug.Log("플레이어 사망! (프로토타입: 게임오버 없음)");
            // 3단계에서 게임오버 UI 추가 예정
        }
    }
}
```

### Step 2: Player GameObject에 컴포넌트 추가

1. Hierarchy → Player Capsule 선택
2. Inspector → Add Component → `PlayerCombat` 추가
3. 수치 확인: Max Hp=100, Attack Damage=10, Attack Cooldown=1.5, Attack Range=2

### Step 3: Play 모드 검증

1. Play 버튼
2. Enemy Capsule 클릭
3. Console에 "플레이어 공격! (데미지: 10)" 로그 1.5초마다 출력 확인
4. 3번 공격 후 (30 데미지) Enemy가 사라지면 성공

### Step 4: 커밋

```bash
cd /Users/harvey/workspace
GIT_TRACE=1 git add "game/Tales of Valeria/Assets/Scripts/Character/PlayerCombat.cs"
GIT_TRACE=1 git commit -m "feat: PlayerCombat.cs — 자동 공격 루프 + 플레이어 HP"
```

---

## Task 4: EnemyAI.cs — 상태 머신 (Idle / Chase / Attack)

**Files:**
- Create: `Assets/Scripts/Enemy/EnemyAI.cs`

**의존성:** PlayerCombat.cs (Task 3 완료 후)

**전제 조건:**
- Player GameObject에 `Player` 태그 설정 필요
  - Hierarchy → Player 선택 → Inspector 상단 Tag 드롭다운 → `Player` 선택

### Step 1: 스크립트 생성

```csharp
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 AI 상태 머신:
///   Idle  → 감지 범위(10m) 내 플레이어 발견 → Chase
///   Chase → 플레이어 추적, 공격 범위(2m) 내 진입 → Attack
///   Attack → 1.5초마다 플레이어 공격, 범위 벗어나면 → Chase
/// </summary>
public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }

    [Header("AI 수치")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public int attackDamage = 5;
    public float attackCooldown = 1.5f;

    private State _state = State.Idle;
    private Transform _player;
    private PlayerCombat _playerCombat;
    private NavMeshAgent _agent;
    private float _attackTimer;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 3.5f;
    }

    void Start()
    {
        // "Player" 태그로 플레이어 자동 탐색
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerCombat = playerObj.GetComponent<PlayerCombat>();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Player 태그를 가진 오브젝트를 찾지 못했습니다!");
        }
    }

    void Update()
    {
        if (_player == null) return;

        switch (_state)
        {
            case State.Idle:   UpdateIdle();   break;
            case State.Chase:  UpdateChase();  break;
            case State.Attack: UpdateAttack(); break;
        }
    }

    private void UpdateIdle()
    {
        if (Vector3.Distance(transform.position, _player.position) <= detectionRange)
        {
            _state = State.Chase;
            Debug.Log($"{gameObject.name}: Idle → Chase");
        }
    }

    private void UpdateChase()
    {
        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist > detectionRange)
        {
            _state = State.Idle;
            _agent.SetDestination(transform.position); // 제자리 정지
            Debug.Log($"{gameObject.name}: Chase → Idle");
            return;
        }

        if (dist <= attackRange)
        {
            _state = State.Attack;
            _agent.SetDestination(transform.position); // 제자리 정지
            _attackTimer = 0f; // 즉시 첫 공격
            Debug.Log($"{gameObject.name}: Chase → Attack");
            return;
        }

        _agent.SetDestination(_player.position);
    }

    private void UpdateAttack()
    {
        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist > attackRange)
        {
            _state = State.Chase;
            Debug.Log($"{gameObject.name}: Attack → Chase");
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            _playerCombat?.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} 공격! (데미지: {attackDamage})");
            _attackTimer = attackCooldown;
        }
    }

    // Unity 에디터에서 감지/공격 범위를 노란/빨간 원으로 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
```

### Step 2: Enemy GameObject 설정

1. Hierarchy → Enemy Capsule 선택
2. Inspector → Add Component:
   - `NavMeshAgent` 추가
   - `EnemyAI` 추가
3. EnemyAI 수치 확인: Detection Range=10, Attack Range=2, Attack Damage=5, Attack Cooldown=1.5
4. NavMeshAgent Speed는 EnemyAI.Awake()에서 3.5로 자동 설정됨

### Step 3: Player 태그 설정 확인

1. Hierarchy → Player Capsule 선택
2. Inspector 상단 → Tag 드롭다운 → `Player` 선택
3. (없으면 Add Tag → 새 태그 `Player` 추가)

### Step 4: Play 모드 검증

1. Play 버튼
2. 플레이어를 Enemy 근처(10m 이내)로 클릭 이동
3. Console에 `Enemy: Idle → Chase` 출력 확인
4. Enemy가 플레이어에게 접근
5. 2m 이내 진입 시 `Enemy: Chase → Attack` 출력
6. 이후 1.5초마다 `플레이어 HP: XX/100` 출력 확인

### Step 5: 커밋

```bash
cd /Users/harvey/workspace
GIT_TRACE=1 git add "game/Tales of Valeria/Assets/Scripts/Enemy/EnemyAI.cs"
GIT_TRACE=1 git commit -m "feat: EnemyAI.cs — 상태 머신 Idle/Chase/Attack"
```

---

## Task 5: Unity Editor 씬 설정 (수동 작업)

> **이 Task는 Unity Editor에서 직접 수행합니다. Claude가 대신 할 수 없습니다.**

### Step 1: NavMesh Bake

1. Window → AI → Navigation (Obsolete) 열기
   - 없으면: Package Manager → AI Navigation 패키지 설치
2. Bake 탭 클릭
3. Bake 버튼 클릭
4. Plane 위에 파란색 영역이 생기면 성공

> **Unity 6 주의:** Navigation 창이 없으면 `Package Manager → Unity Registry → AI Navigation` 설치

### Step 2: Layer 설정

1. Edit → Project Settings → Tags and Layers
2. 비어있는 User Layer에 추가:
   - `Ground` (예: Layer 6)
   - `Enemy` (예: Layer 7)
3. Hierarchy → Plane 선택 → Inspector 상단 Layer → `Ground`
4. Hierarchy → Enemy Capsule들 선택 → Inspector 상단 Layer → `Enemy`

### Step 3: 씬 구성 확인

| GameObject | 위치 | 컴포넌트 |
|-----------|------|---------|
| Plane (바닥) | (0,0,0), Scale(2,1,2) | MeshRenderer, Ground 레이어 |
| Player Capsule | (0,0.5,0) | NavMeshAgent, PlayerController, PlayerCombat, **Player 태그** |
| Enemy Capsule x2~3 | 플레이어 주변 5~8m | NavMeshAgent, EnemyAI, EnemyHealth, Enemy 레이어 |
| Main Camera | - | IsometricCamera.cs, Target = Player |

### Step 4: 최종 Play 테스트

1. ▶ Play 버튼
2. **이동 테스트:** 바닥 클릭 → 플레이어 이동
3. **전투 테스트:** 적 클릭 → 자동 공격 → 3번 히트 후 적 소멸
4. **AI 테스트:** 적이 플레이어를 감지하고 추적 → 공격
5. Console 에러 없음 확인

**예상 Console 출력:**
```
Enemy(1): Idle → Chase
Enemy(1): Chase → Attack
플레이어 HP: 95/100
Enemy(1) HP: 20/30
플레이어 공격! (데미지: 10)
Enemy(1) HP: 10/30
플레이어 공격! (데미지: 10)
Enemy(1) HP: 0/30
Enemy(1) 사망!
```

---

## Task 6: CLAUDE.md 업데이트 + 최종 커밋

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/DEVELOPMENT_ROADMAP.md`

### Step 1: CLAUDE.md 현재 단계 업데이트

`CLAUDE.md`의 "현재 단계" 섹션을 수정:

```markdown
## 현재 단계

**2단계 프로토타입 구현 완료**
```

"다음 할 일" 섹션을 수정:

```markdown
## 다음 할 일

1. HP 바 UI 추가 (Canvas + Slider)
2. 애니메이션 연결 (Animator 컴포넌트)
3. Enemy Prefab 생성 (재사용 가능한 적 템플릿)
```

### Step 2: docs/DEVELOPMENT_ROADMAP.md 2단계 완료 체크

2단계 항목들을 완료 체크:

```markdown
## 2단계: 프로토타입 MVP

- [x] PlayerController.cs: 클릭 이동 (NavMesh)
- [x] PlayerCombat.cs: 적 클릭 → 자동 공격 (1.5초 쿨다운)
- [x] EnemyHealth.cs: 적 HP + 사망 처리
- [x] EnemyAI.cs: 상태 머신 (Idle → Chase → Attack)
```

### Step 3: 최종 커밋

```bash
cd /Users/harvey/workspace
GIT_TRACE=1 git add \
  "game/Tales of Valeria/CLAUDE.md" \
  "game/Tales of Valeria/docs/DEVELOPMENT_ROADMAP.md"
GIT_TRACE=1 git commit -m "docs: 2단계 프로토타입 완료 — CLAUDE.md + 로드맵 업데이트"
```

---

## 검증 체크리스트

- [ ] 바닥 클릭 시 플레이어가 NavMesh를 따라 이동함
- [ ] 적 클릭 시 플레이어가 접근 후 1.5초마다 자동 공격
- [ ] 3번 공격 후 적 HP ≤ 0 → 적 소멸
- [ ] 적이 플레이어를 감지하고 추적
- [ ] 적이 플레이어에게 1.5초마다 5 데미지
- [ ] Console에 에러(빨간 로그) 없음
- [ ] "이게 일렌시아처럼 느껴지는가?" → YES

---

## 다음 단계 (3단계 프리뷰)

- HP 바 UI (Canvas + Slider)
- 애니메이션 (Animator, Animation Clip)
- Enemy Prefab (재사용 가능한 적 템플릿)
- 게임오버 / 승리 조건
