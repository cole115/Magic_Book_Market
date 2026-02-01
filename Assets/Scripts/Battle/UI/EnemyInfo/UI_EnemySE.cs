using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_EnemySE : MonoBehaviour
{
    [SerializeField] GameObject burnIcon;
    [SerializeField] GameObject freezeIcon;
    [SerializeField] GameObject poisonIcon;
    [SerializeField] GameObject shockIcon;

    private Dictionary<StatusEffectType, GameObject> iconMap;

    private void Awake()
    {
        iconMap = new Dictionary<StatusEffectType, GameObject>()
        {
            {StatusEffectType.Burn, burnIcon },
            {StatusEffectType.Freeze, freezeIcon},
            {StatusEffectType.Poison, poisonIcon},
            {StatusEffectType.Shock, shockIcon}
        };

    }

    // 적 상태이상 UI로 보여줌
    public void ShowSE(Dictionary<StatusEffectType, int> effects)
    {
        foreach (var pair in iconMap)
        {
            StatusEffectType type = pair.Key;
            GameObject icon = pair.Value;

            // 특정 상태이상 스택이 0보다 크면 아이콘 적용.
            // 아니면 아이콘 비활성화
            if (effects.TryGetValue(type, out var value) && value > 0)
            {
                ApplyIcon(icon, value);
            }
            else
            {
                icon.SetActive(false);
            }
        }

    }

    // 아이콘 및 스택 활성화
    private void ApplyIcon(GameObject icon, int value)
    {
        if (!icon.activeSelf)
            icon.SetActive(true);

        var text = icon.GetComponentInChildren<TextMeshProUGUI>();
        text.text = value.ToString();

    }

}
