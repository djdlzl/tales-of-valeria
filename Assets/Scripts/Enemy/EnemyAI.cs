using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 기본 적 AI:
///   - 감지 반경(detectionRange) 내 플레이어 진입 시 추적 시작
///   - 공격 범위(attackRange) 내 진입 시 자동 공격 (PlayerCombat.TakeDamage 호출)
///   - 플레이어가 감지 반경 밖으로 나가면 원위치로 복귀
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("감지 / 공격 수치")]
    public float detectionRange = 8f;
    public float attackRange = 1.8f;
    public int attackDamage = 5;
    public float attackCooldown = 1.5f;

    private NavMeshAgent _agent;
    private PlayerCombat _playerCombat;
    private Transform _playerTransform;

    private Vector3 _originPosition;
    private float _attackTimer;

    private enum State { Idle, Chase, Attack, Return }
    private State _state = State.Idle;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _originPosition = transform.position;
    }

    void Start()
    {
        // 씬에서 플레이어 오브젝트 탐색 (Player 레이어 기준)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
            _playerCombat = player.GetComponent<PlayerCombat>();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (_playerTransform == null) return;

        float distToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        switch (_state)
        {
            case State.Idle:
                if (distToPlayer <= detectionRange)
                {
                    _state = State.Chase;
                }
                break;

            case State.Chase:
                if (distToPlayer <= attackRange)
                {
                    _agent.SetDestination(transform.position); // 정지
                    _state = State.Attack;
                }
                else if (distToPlayer > detectionRange)
                {
                    _state = State.Return;
                }
                else
                {
                    _agent.SetDestination(_playerTransform.position);
                }
                break;

            case State.Attack:
                _agent.SetDestination(transform.position); // 공격 중 정지 유지

                // 플레이어가 공격 범위 밖으로 빠지면 다시 추적
                if (distToPlayer > attackRange)
                {
                    _state = State.Chase;
                    break;
                }

                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0f)
                {
                    PerformAttack();
                    _attackTimer = attackCooldown;
                }
                break;

            case State.Return:
                _agent.SetDestination(_originPosition);

                float distToOrigin = Vector3.Distance(transform.position, _originPosition);
                if (distToOrigin < 0.5f)
                {
                    _agent.SetDestination(transform.position);
                    _state = State.Idle;
                }
                else if (distToPlayer <= detectionRange)
                {
                    _state = State.Chase; // 복귀 중 다시 감지
                }
                break;
        }
    }

    private void PerformAttack()
    {
        if (_playerCombat == null) return;

        _playerCombat.TakeDamage(attackDamage);
        Debug.Log($"{gameObject.name} 공격! (데미지: {attackDamage})");
    }

    // 씬 뷰에서 감지/공격 반경 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
