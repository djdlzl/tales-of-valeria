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
