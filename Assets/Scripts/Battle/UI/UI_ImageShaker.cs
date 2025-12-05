using System.Collections;
using UnityEngine;

public class UI_ImageShaker : MonoBehaviour
{
    [SerializeField] RectTransform target;
    [SerializeField] float duration = 0.3f;
    [SerializeField] float magnitude = 10f;

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = target.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            target.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;

        }

        target.anchoredPosition = originalPos;
    }
}
