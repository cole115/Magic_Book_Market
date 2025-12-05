using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHpBar : MonoBehaviour
{
    [SerializeField] Image BarImage;
    [SerializeField] TextMeshProUGUI HpText;
    
    public void ShowEnemyHp(int currHp, int maxHp)
    {
        BarImage.fillAmount = (float)currHp / maxHp;
        HpText.text = currHp.ToString();
    }
}
