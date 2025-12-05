using UnityEngine;
using System.Collections;
using TMPro;

public class UI_EffectText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] RectTransform rect;
    [SerializeField] CanvasGroup group;



    [SerializeField] float duration = 0.8f;
    [SerializeField] float moveDistance = 60f;

    public IEnumerator DisplayText(string message)
    {
        text.text = message;

        float t = 0;

        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;
        Vector2 endPos = startPos + Vector2.up * moveDistance;
        

        while (t < duration)
        {
            t += Time.deltaTime / duration; 
            rect.anchoredPosition = Vector2.Lerp(startPos,endPos,MyEase.EaseOutQuad(t));
            //rect.localScale = startScale * Ease.EaseOutBack(t);
            
            group.alpha = 1f - MyEase.EaseOutCubic(t);
            yield return null;

        }

        Destroy(gameObject);
    }
}
