using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerNameManager : MonoBehaviour
{
    public InputField nameInputField;  // 유저가 이름 입력하는 UI
    public Text playerNameText;        // 화면에 표시되는 플레이어 이름

    private List<string> bannedWords = new List<string>()
    {
        "fuck", "shit", "개새", "시발", "욕설", "admin", "gm"  // 금칙어 리스트 예시
    };

    private const int minNameLength = 2;
    private const int maxNameLength = 12;

    void Start()
    {
        // 기존 이름 로드
        string savedName = PlayerPrefs.GetString("PlayerName", "Player");
        playerNameText.text = savedName;
    }

    public void OnClickChangeNameButton()
    {
        nameInputField.gameObject.SetActive(true);  // 입력 UI 열기
    }

    public void OnClickConfirmName()
    {
        string newName = nameInputField.text.Trim();

        // 1. 빈 문자열 체크
        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("이름이 비어있습니다.");
            return;
        }

        // 2. 글자 수 제한
        if (newName.Length < minNameLength || newName.Length > maxNameLength)
        {
            Debug.Log($"이름은 {minNameLength}~{maxNameLength} 글자여야 합니다.");
            return;
        }

        // 3. 금칙어 체크
        foreach (string badWord in bannedWords)
        {
            if (newName.ToLower().Contains(badWord))
            {
                Debug.Log("금칙어가 포함되어 있습니다.");
                return;
            }
        }

        // 4. 문제 없으면 저장
        PlayerPrefs.SetString("PlayerName", newName);
        playerNameText.text = newName;

        // UI 닫기
        nameInputField.gameObject.SetActive(false);
    }
}
