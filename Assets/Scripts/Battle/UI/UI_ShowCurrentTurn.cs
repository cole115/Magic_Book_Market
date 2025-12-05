using TMPro;
using UnityEngine;

public class UI_ShowCurrentTurn : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turnText;


    public void SetTurnText(string text)
    {
        turnText.text = text;
    }
}
