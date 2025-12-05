using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TypingEffect : MonoBehaviour
{
    public Text uiText;          // 출력할 UI Text
    [TextArea] public string fullText; // 전체 문장
    public float typingSpeed = 0.05f; // 글자 출력 속도

    private Coroutine typingCoroutine;

    void Start()
    {
        // 자동 실행 원하면 Start에서 실행
        StartTyping();
    }

    // 타이핑 시작
    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypingRoutine());
    }

    IEnumerator TypingRoutine()
    {
        uiText.text = "";  // 초기화

        for (int i = 0; i < fullText.Length; i++)
        {
            uiText.text += fullText[i];  // 한 글자씩 추가
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // 즉시 전체 텍스트 출력 (스킵 기능)
    public void ShowAll()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        uiText.text = fullText;
    }
}
