using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CardFlipArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("카드 뒤집기");

        var uiCard = eventData.pointerDrag?.GetComponent<UI_HandCard>();
        if (uiCard == null)
            return;

        bool canFlip = uiCard.CanFlipCard();

        if(!canFlip)
        {
            uiCard.ResetCardPos();
            return;
        }

        PlayAudio();
        StartCoroutine(uiCard.FlipCard());
    }


    private void PlayAudio()
    {
        var container = GetComponent<AudioSourceContainer>();

        container.PlayAudio(0);
    }
}
