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


    public void ShowSE(Dictionary<StatusEffectType, int> effects)
    {
        foreach (var pair in iconMap)
        {
            StatusEffectType type = pair.Key;
            GameObject icon = pair.Value;

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

    private void ApplyIcon(GameObject icon, int value)
    {
        if (!icon.activeSelf)
            icon.SetActive(true);

        var text = icon.GetComponentInChildren<TextMeshProUGUI>();
        text.text = value.ToString();

    }

}
