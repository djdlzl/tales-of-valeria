using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 숙련도 시스템 — 플레이어의 무기별 숙련도를 추적하고 역할을 자동 배정.
///
/// 사용법:
///   proficiencySystem.AddExp(WeaponType.Sword, config.expPerAttack);
///   proficiencySystem.AddExp(WeaponType.Sword, config.expPerKill);  // 적 처치 시
///
/// 이벤트:
///   OnLevelUp: 레벨업 시 (WeaponType, newLevel)
///   OnRoleQualified: 역할 자격 획득 시 (WeaponType)
///   OnMainRoleChanged: 주 역할 변경 시 (WeaponType)
/// </summary>
public class ProficiencySystem : MonoBehaviour
{
    [Header("설정")]
    public ProficiencyConfig config;

    // 이벤트 — UI, 사운드, 이펙트 등에서 구독
    public event Action<WeaponType, int> OnLevelUp;
    public event Action<WeaponType> OnRoleQualified;
    public event Action<WeaponType> OnMainRoleChanged;

    /// <summary>무기별 숙련도 데이터</summary>
    [Serializable]
    public class ProficiencyData
    {
        public int level = 1;
        public int currentExp;
        public bool roleQualified;
    }

    // 내부 상태
    private Dictionary<WeaponType, ProficiencyData> _proficiencies = new();
    private WeaponType? _mainRole;

    /// <summary>현재 주 역할 (가장 높은 숙련도). null이면 아직 역할 없음.</summary>
    public WeaponType? MainRole => _mainRole;

    void Awake()
    {
        // 모든 무기 타입에 대해 초기 데이터 생성
        foreach (WeaponType weapon in Enum.GetValues(typeof(WeaponType)))
        {
            _proficiencies[weapon] = new ProficiencyData();
        }
    }

    /// <summary>숙련도 경험치 추가. 레벨업·역할 판정 자동 처리.</summary>
    public void AddExp(WeaponType weapon, int amount)
    {
        if (config == null)
        {
            Debug.LogWarning("ProficiencySystem: config이 할당되지 않았습니다!");
            return;
        }

        var data = _proficiencies[weapon];
        data.currentExp += amount;

        // 레벨업 체크 (연속 레벨업 가능)
        while (data.level < config.MaxLevel)
        {
            int required = config.GetExpForLevel(data.level + 1);
            if (data.currentExp < required) break;

            data.currentExp -= required;
            data.level++;

            string roleName = GetRoleName(weapon);
            Debug.Log($"[숙련도] {roleName} Lv.{data.level} 달성!");
            OnLevelUp?.Invoke(weapon, data.level);

            // 역할 자격 판정
            if (!data.roleQualified && data.level >= config.roleQualifyLevel)
            {
                data.roleQualified = true;
                Debug.Log($"[숙련도] ★ {roleName} 역할 자격 획득!");
                OnRoleQualified?.Invoke(weapon);
            }
        }

        // 주 역할 갱신 — 자격 있는 것 중 가장 높은 레벨
        UpdateMainRole();
    }

    /// <summary>특정 무기의 숙련도 데이터 조회</summary>
    public ProficiencyData GetData(WeaponType weapon)
    {
        return _proficiencies[weapon];
    }

    /// <summary>특정 무기의 역할 자격 여부</summary>
    public bool IsRoleQualified(WeaponType weapon)
    {
        return _proficiencies[weapon].roleQualified;
    }

    private void UpdateMainRole()
    {
        WeaponType? best = null;
        int bestLevel = 0;

        foreach (var kvp in _proficiencies)
        {
            if (kvp.Value.roleQualified && kvp.Value.level > bestLevel)
            {
                best = kvp.Key;
                bestLevel = kvp.Value.level;
            }
        }

        if (best != _mainRole)
        {
            _mainRole = best;
            if (_mainRole.HasValue)
            {
                Debug.Log($"[숙련도] 주 역할 변경 → {GetRoleName(_mainRole.Value)}");
                OnMainRoleChanged?.Invoke(_mainRole.Value);
            }
        }
    }

    /// <summary>WeaponType → 한글 역할 이름</summary>
    public static string GetRoleName(WeaponType weapon)
    {
        return weapon switch
        {
            WeaponType.Sword => "검사",
            WeaponType.Bow => "궁수",
            _ => weapon.ToString()
        };
    }
}
