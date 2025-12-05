using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScaleChanger_2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] Image btnImg;
    Vector3 originSize;

    private void Awake()
    {
        btnImg = GetComponentInChildren<Image>();
        originSize = btnImg.rectTransform.localScale;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        btnImg.rectTransform.localScale = originSize * 0.7f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        btnImg.rectTransform.localScale = originSize;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        btnImg.rectTransform.localScale = originSize;
    }
}