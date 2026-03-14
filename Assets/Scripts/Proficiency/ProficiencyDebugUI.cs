using System;
using UnityEngine;

/// <summary>
/// 숙련도 디버그 UI — 화면 좌상단에 실시간 숙련도 정보 표시.
/// Player 오브젝트에 붙이면 자동으로 ProficiencySystem을 찾아서 표시.
/// 빌드 시 제거하거나 showDebugUI 체크 해제.
/// </summary>
public class ProficiencyDebugUI : MonoBehaviour
{
    [Header("디버그 설정")]
    public bool showDebugUI = true;
    public int fontSize = 16;

    private ProficiencySystem _proficiency;
    private GUIStyle _boxStyle;
    private GUIStyle _labelStyle;
    private GUIStyle _headerStyle;
    private string _lastEvent = "";
    private float _eventTimer;

    void Awake()
    {
        _proficiency = GetComponent<ProficiencySystem>();
    }

    void Start()
    {
        if (_proficiency == null) return;

        // 이벤트 구독 — 레벨업, 역할 획득 시 알림 표시
        _proficiency.OnLevelUp += (weapon, level) =>
        {
            _lastEvent = $"★ {ProficiencySystem.GetRoleName(weapon)} Lv.{level} 달성!";
            _eventTimer = 3f;
        };

        _proficiency.OnRoleQualified += (weapon) =>
        {
            _lastEvent = $"★★ {ProficiencySystem.GetRoleName(weapon)} 역할 자격 획득!";
            _eventTimer = 5f;
        };

        _proficiency.OnMainRoleChanged += (weapon) =>
        {
            _lastEvent = $">>> 주 역할 변경: {ProficiencySystem.GetRoleName(weapon)}";
            _eventTimer = 5f;
        };
    }

    void Update()
    {
        if (_eventTimer > 0) _eventTimer -= Time.deltaTime;
    }

    void OnGUI()
    {
        if (!showDebugUI || _proficiency == null || _proficiency.config == null) return;

        InitStyles();

        float x = 10f;
        float y = 10f;
        float width = 260f;
        float lineHeight = fontSize + 6f;

        // 배경 박스
        int lineCount = 3; // 헤더 + 무기 2개
        if (_proficiency.MainRole.HasValue) lineCount++;
        if (_eventTimer > 0) lineCount++;
        float boxHeight = lineHeight * lineCount + 20f;

        GUI.Box(new Rect(x, y, width, boxHeight), "", _boxStyle);
        y += 6f;

        // 헤더
        GUI.Label(new Rect(x + 8, y, width, lineHeight), "[ 숙련도 ]", _headerStyle);
        y += lineHeight;

        // 주 역할 표시
        if (_proficiency.MainRole.HasValue)
        {
            string roleName = ProficiencySystem.GetRoleName(_proficiency.MainRole.Value);
            GUI.Label(new Rect(x + 8, y, width, lineHeight), $"주 역할: {roleName}", _headerStyle);
            y += lineHeight;
        }

        // 각 무기 숙련도
        foreach (WeaponType weapon in Enum.GetValues(typeof(WeaponType)))
        {
            var data = _proficiency.GetData(weapon);
            string roleName = ProficiencySystem.GetRoleName(weapon);
            string qualified = data.roleQualified ? " ✓" : "";

            // 다음 레벨 필요 경험치
            string expText;
            if (data.level >= _proficiency.config.MaxLevel)
            {
                expText = "MAX";
            }
            else
            {
                int needed = _proficiency.config.GetExpForLevel(data.level + 1);
                expText = $"{data.currentExp}/{needed}";
            }

            GUI.Label(new Rect(x + 8, y, width, lineHeight),
                $"{roleName} Lv.{data.level} ({expText}){qualified}", _labelStyle);
            y += lineHeight;
        }

        // 이벤트 알림
        if (_eventTimer > 0)
        {
            GUI.Label(new Rect(x + 8, y, width, lineHeight), _lastEvent, _headerStyle);
        }
    }

    private void InitStyles()
    {
        if (_boxStyle != null) return;

        _boxStyle = new GUIStyle(GUI.skin.box);
        _boxStyle.normal.background = MakeTexture(2, 2, new Color(0f, 0f, 0f, 0.7f));

        _labelStyle = new GUIStyle(GUI.skin.label);
        _labelStyle.fontSize = fontSize;
        _labelStyle.normal.textColor = Color.white;

        _headerStyle = new GUIStyle(GUI.skin.label);
        _headerStyle.fontSize = fontSize;
        _headerStyle.fontStyle = FontStyle.Bold;
        _headerStyle.normal.textColor = new Color(1f, 0.85f, 0.3f); // 금색
    }

    private static Texture2D MakeTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        var tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
