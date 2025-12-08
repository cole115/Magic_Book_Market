using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_EnemyAttackResult : MonoBehaviour
{
    [Header("OnBreak")]
    [SerializeField] private GameObject BrokenCard;

    [Header("OnEvade & OnCorrupted")]
    [SerializeField] private GameObject TextPrefab;



    // 공격 회피
    public void OnEvade(Vector3 cardPos)
    {
        Debug.Log("회피");
        string result = "회피!";

        GenerateText(cardPos, result);
        PlayAudio(0);
    }


    // 카드 깨짐 연출(나중에 애니메이터로 깨지는 형태 연출)
    public IEnumerator OnBreak(UI_HandCard handCardUI)
    {
        //Debug.Log("파괴");
        RectTransform rect = BrokenCard.GetComponent<RectTransform>();


        rect.position = handCardUI.rect.position;
        rect.rotation = handCardUI.rect.rotation;

        BrokenCard.SetActive(true);        
        var cardImage = BrokenCard.GetComponent<Image>();

        cardImage.sprite = handCardUI.image.sprite;
        PlayAudio(1);
        yield return new WaitForSeconds(0.3f);

        // 카드 슬롯의 카드 뺌
        handCardUI.image.sprite = null;
        BrokenCard.SetActive(false);

    }

    // 오염도 증가
    public void OnCorrupted(Vector3 cardPos, int amount)
    {
        Debug.Log("오염");
        string result = $"{amount} 오염";

        GenerateText(cardPos, result);
        PlayAudio(2);

    }

    private void GenerateText(Vector3 cardPos, string message)
    {
        var textPrefab = Instantiate(TextPrefab, cardPos, Quaternion.identity,transform);
        var uiText = textPrefab.GetComponent<UI_EffectText>();
        StartCoroutine(uiText.DisplayText(message));

    }


    private void PlayAudio(int index)
    {
        AudioSourceContainer container = GetComponent<AudioSourceContainer>();

        container.PlayAudio(index);
    }
}
