using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScaleChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerExitHandler
{
    Image btnImg;
    TextMeshProUGUI btnText;
    Vector3 originSize;
    float originFont;

    private void OnEnable()
    {
        btnImg = GetComponentInChildren<Image>();
        btnText = GetComponentInChildren<TextMeshProUGUI>();
        originSize = btnImg.rectTransform.localScale;
        
        if(btnText != null )
            originFont = btnText.fontSize;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        
        btnImg.rectTransform.localScale = originSize * 0.7f;
        ResizeText(0.7f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        btnImg.rectTransform.localScale = originSize;
        ResizeText(1.0f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        btnImg.rectTransform.localScale = originSize;
        ResizeText(1.0f);
    }

    private void ResizeText(float changeAmount)
    {
        if (btnText != null)
        {
            btnText.fontSize = originFont * changeAmount;
        }
    }
}
