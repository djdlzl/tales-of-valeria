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
