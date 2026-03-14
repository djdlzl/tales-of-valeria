using UnityEngine;

/// <summary>
/// 숙련도 설정 ScriptableObject.
/// Inspector에서 레벨 커브, 경험치 획득량 등을 조절 가능.
/// [Create] Assets > Create > Valeria > ProficiencyConfig
/// </summary>
[CreateAssetMenu(fileName = "ProficiencyConfig", menuName = "Valeria/ProficiencyConfig")]
public class ProficiencyConfig : ScriptableObject
{
    [Header("레벨별 필요 경험치")]
    [Tooltip("인덱스 = 목표 레벨. expPerLevel[2] = Lv.1→Lv.2에 필요한 경험치")]
    public int[] expPerLevel = { 0, 0, 100, 250, 500, 1000 };
    // Lv.0=없음, Lv.1=시작(0), Lv.2=100, Lv.3=250, Lv.4=500, Lv.5=1000

    [Header("경험치 획득량")]
    [Tooltip("일반 공격 1회당 숙련도 경험치")]
    public int expPerAttack = 5;

    [Tooltip("적 처치 시 보너스 경험치")]
    public int expPerKill = 20;

    [Header("역할 배정")]
    [Tooltip("이 레벨 이상이면 해당 역할 자격 획득")]
    public int roleQualifyLevel = 3;

    /// <summary>해당 레벨의 최대 레벨인지 확인</summary>
    public int MaxLevel => expPerLevel.Length - 1;

    /// <summary>목표 레벨에 도달하려면 필요한 경험치</summary>
    public int GetExpForLevel(int level)
    {
        if (level < 0 || level >= expPerLevel.Length) return int.MaxValue;
        return expPerLevel[level];
    }
}
