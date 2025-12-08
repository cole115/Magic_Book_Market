using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UI_CardExecuteArea : MonoBehaviour, IDropHandler
{
    [SerializeField] Transform executedCard;
    [SerializeField] float moveDistance = 80f;      // 카드가 올라가는 거리
    [SerializeField] float fadeInDuration = 0.15f;  // 카드가 페이드 인 되는 시간
    [SerializeField] float riseDuration = 0.35f;    // 카드가 올라가는 시간
    //[SerializeField] float holdDurtaion = 0.6f; // 화면에 카드가 멈춘 채로 남는 시간
    [SerializeField] float fadeOutDuration = 0.25f; // 카드가 페이드 아웃 되는 시간
    [SerializeField] float popScaleMultiplier = 1.25f; // 카드가 확대되는 크기
    [SerializeField] float popDuration = 0.18f; // 카드가 확대되는 시간

    private RectTransform cardRect;
    private Sprite handSprite;
    private Image cardImage;
    private CanvasGroup group;
    private Vector2 startPos;
    private Vector3 initialScale;
    private Image thisImage;

    private bool isPlaying = false;

    private void Awake()
    {
        cardRect = executedCard.GetComponent<RectTransform>();
        cardImage = executedCard.GetComponent<Image>();
        group = executedCard.GetComponent<CanvasGroup>();
        startPos = cardRect.anchoredPosition;
        initialScale = cardRect.localScale;
        thisImage = GetComponent<Image>();

        ResetExecutedCard();
    }





    public void OnDrop(PointerEventData eventData)
    {
        if (isPlaying) return;

        var uiCard = eventData.pointerDrag?.GetComponent<UI_HandCard>();
        if (uiCard == null) return;

        handSprite = uiCard.image.sprite;


        // 효과를 쓸 수 있는지 체크. 가능하다면 사용
        bool canUse = uiCard.CanUseCard();

        if (!canUse)
        {
            uiCard.ResetCardPos();
            return;
        }
        Debug.Log("카드 사용");

        PlayCardAnimation(uiCard);
        //StartCoroutine(ActivateCardCoroutine(uiCard));

    }
    private void PlayCardAnimation(UI_HandCard handCard)
    {
        PlayAudio();


        isPlaying = true;

        thisImage.raycastTarget = false;
        cardImage.sprite = handSprite;
        cardRect.anchoredPosition = startPos;
        cardRect.localScale = initialScale;
        group.alpha = 0f;
        

        Vector2 endPos = startPos + Vector2.up * moveDistance;

        // DOTween Sequence 생성
        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad));

        seq.Join(cardRect.DOAnchorPos(endPos, riseDuration).SetEase(Ease.OutQuad));
        seq.Join(cardRect.DOScale(initialScale * popScaleMultiplier, popDuration).SetEase(Ease.OutBack));

        seq.Append(group.DOFade(0f, fadeOutDuration).SetEase(Ease.OutQuad));

        seq.OnComplete(() =>
        {
            handCard.EmptyCard();
            ResetExecutedCard();
            thisImage.raycastTarget = true;
            isPlaying = false;
        });

    }





    private void ResetExecutedCard()
    {
        cardRect.anchoredPosition = startPos;
        cardRect.localScale = initialScale;
        group.alpha = 0;
        cardImage.sprite = null;
    }

    private void PlayAudio()
    {
        var container = GetComponent<AudioSourceContainer>();

        container.PlayAudio(0);
    }
}
