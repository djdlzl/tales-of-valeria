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
