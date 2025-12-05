using UnityEngine;
using UnityEngine.UI;

public class UI_MiasmaBar : MonoBehaviour
{
    [SerializeField] Image BarImage;
    
    public void ShowCurrentMiasma(int currMiasma, int maxMiasma)
    {
        BarImage.fillAmount = (float)currMiasma / maxMiasma;
    }
}
