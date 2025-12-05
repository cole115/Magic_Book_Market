using TMPro;
using UnityEngine;

public class UI_RerollText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rerollText;

    public void ShowCurrentReroll(int reroll)
    {
        rerollText.text = $"X {reroll}";    
    }
}

