using UnityEngine;
using UnityEngine.EventSystems;

public class UIScrollMover : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform content;   // 움직일 UI (긴 이미지)
    public float dragSpeed = 1.0f;  // 드래그 민감도

    public float minY = -500f;  // 아래로 이동 제한
    public float maxY = 500f;   // 위로 이동 제한

    private Vector2 lastPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = content.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = content.anchoredPosition;

        // 드래그한 만큼 이동
        pos.y += eventData.delta.y * dragSpeed;

        // 위치 제한 (Clamp)
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        content.anchoredPosition = pos;
    }
}
