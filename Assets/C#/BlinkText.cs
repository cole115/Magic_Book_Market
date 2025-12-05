using UnityEngine;
using UnityEngine.UI;

public class FadeBlinkText : MonoBehaviour
{
    public Text uiText;
    public float speed = 2f;   // 깜빡이는 속도

    private Color baseColor;

    private void Awake()
    {
        if (uiText == null)
            uiText = GetComponent<Text>();

        baseColor = uiText.color;
    }

    private void Update()
    {
        // 0~1 사이로 알파가 반복해서 변함
        float a = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        uiText.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
    }
}
