using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas parentCanvas;
    public Transform targetA;          // A 슬롯 Content
    public Transform targetB;          // B 슬롯 Content

    private CanvasGroup canvasGroup;
    private Transform parentBeforeDrag;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (parentCanvas == null)
            parentCanvas = FindObjectOfType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(parentCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        //  여기서 "어디로 갈지" 그냥 지정하면 끝
        if (parentBeforeDrag == targetA)
        {
            // A에서 끌었다 → B로
            transform.SetParent(targetB, false);
        }
        else
        {
            // B에서 끌었다 → A로
            transform.SetParent(targetA, false);
        }
    }
}
