using UnityEngine;
using TMPro;

public class UI_ManaText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI manaText;

    public void ShowCurrentMana(int currentMana, int maxMana)
    {
        manaText.text = $"{currentMana} / {maxMana}";
    }
}
