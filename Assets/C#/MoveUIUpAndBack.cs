using UnityEngine;
using System.Collections;

public class MoveUIUpAndBack : MonoBehaviour
{
    public RectTransform target;
    public float moveDistance = 200f;
    public float speed = 6f;

    private Vector2 originalPos;
    private Vector2 downPos;
    private bool isMoving = false;

    public void StartMove()
    {
        if (!isMoving)
            StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        isMoving = true;

        //  이동 시작 시마다 현재 위치 기준으로 계산해야 함
        originalPos = target.anchoredPosition;
        downPos = originalPos - new Vector2(0, moveDistance);

        // 아래로 이동
        while (Vector2.Distance(target.anchoredPosition, downPos) > 0.5f)
        {
            target.anchoredPosition = Vector2.Lerp(
                target.anchoredPosition,
                downPos,
                Time.deltaTime * speed
            );
            yield return null;
        }
        target.anchoredPosition = downPos;


        // 위로 복귀
        while (Vector2.Distance(target.anchoredPosition, originalPos) > 0.5f)
        {
            target.anchoredPosition = Vector2.Lerp(
                target.anchoredPosition,
                originalPos,
                Time.deltaTime * speed
            );
            yield return null;
        }
        target.anchoredPosition = originalPos;

        isMoving = false;
    }
}
