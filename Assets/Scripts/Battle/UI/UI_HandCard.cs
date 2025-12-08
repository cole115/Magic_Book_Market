using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 패의 카드의 ui를 관리할 클래스
public class UI_HandCard : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image image {get;private set;}
    public RectTransform rect { get; private set; }

    Sprite placeHolder;
    Transform canvas;
    Transform previousParent;    


    // 카드 사용 시도 이벤트
    // 카드 방향 및 마나 체크하고 가능 여부를 bool로 return
    public event Func<int, bool> OnTryUseCard;
    // 카드 플립 시도 이벤트
    // 마나 체크하고 가능 여부를 bool로 return
    public event Func<int, bool> OnTryFlipCard;

    public event Action<GameCard> OnDescriptionOpen;
    public event Action OnDescriptionClose;


    private GameCard slotCard;    
    private int slotIndex;  // 슬롯 번호는 0~3

    // 입력 이벤트 관련 변수
    private Coroutine longPressRoutine;
    private Vector2 pressStartPos;
    private bool canDrag = true;
    



    private void Awake()
    {
        image = GetComponent<Image>();

        canvas = transform.root;
        rect = GetComponent<RectTransform>();

    }

   
    public void Init(int slotIndex, Sprite placeHolder)
    {
        this.slotIndex = slotIndex;
        this.placeHolder = placeHolder;
    }



    // 플레이스 홀더 보여주기
    // 반투명하게 하니까 슬롯이랑 색이 겹쳐서 진해지므로 그냥 알파값 0으로
    public void ShowPlaceHolder()
    {
        if(placeHolder != null)
        {
            image.sprite = placeHolder;

            Color c = image.color;
            c.a = 0f;
            image.color = c;
        }
        
    }



    //public void ShowCard(Sprite sprite, CardFace face)

    // ui_cardview에서 스프라이트 가져오기
    // 카드 정보는 BattleUIPresenter가 갖고 있음
    // cardFace에 따라 canDrag와 카드 방향이 결정됨
    public void ShowCard(GameCard card)
    {
        slotCard = card;

        image.enabled = true;
        image.sprite = card.Card.CardSprite;

        Color c= image.color;
        c.a = 1f;
        image.color = c;


        HandleDraggable(true);

        UpdateCardFace(slotCard.CardFace);
    }


    private void UpdateCardFace(CardFace face)
    {
        if (face == CardFace.Upright)
        {
            rect.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            rect.localEulerAngles = new Vector3(0, 0, 180);
        }
    }


    // 카드 비움
    public void EmptyCard()
    {
        ResetCardPos(); 
        HandleDraggable(false);
        image.sprite = placeHolder;
        rect.localScale = Vector3.one;  
    }

    // 드래그 가능 여부
    public void HandleDraggable(bool isAble)
    {
        canDrag = isAble;
        image.raycastTarget = isAble;
    }


#region InputEvents




    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        pressStartPos = eventData.position;
        longPressRoutine = StartCoroutine(LongPressCheck());

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopLongPress();
    }


    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        StopLongPress();

        if (!canDrag)
            return;

        previousParent = transform.parent;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        image.raycastTarget = false;

    }
     
    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;
        rect.position = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;
        if (transform.parent == canvas)
        {
            ResetCardPos();
        }

        image.raycastTarget = true;
    }


    private float longPressTime = 0.4f;
    private float dragThreshold = 15f;
    private IEnumerator LongPressCheck()
    {
        float timer = 0f;

        while(timer < longPressTime)
        {
            Vector2 currPos = Pointer.current.position.ReadValue();
            float dist = Vector2.Distance(currPos, pressStartPos);

            if (dist > dragThreshold)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        // 카드 설명 끄기
        OnDescriptionOpen?.Invoke(slotCard);
    }


    private void StopLongPress()
    {
        if (longPressRoutine != null)
        {
            StopCoroutine(longPressRoutine);
            longPressRoutine = null;
        }

        // 카드 설명 켜기
        OnDescriptionClose?.Invoke();
    }

    #endregion





    // 카드를 쓸 수 있는지 체크
    public bool CanUseCard()
    {
        return OnTryUseCard?.Invoke(slotIndex) ?? false;
    }

    public bool CanFlipCard()
    {
        return OnTryFlipCard?.Invoke(slotIndex) ?? false;
    }

    // 패의 카드 뒤집을 시 실행
    // 카드 방향이 뒤집기 전에 이미 정해져 버리도록 만들어져 있으므로, 차라리 현재 방향을 참조해서 그 반대방향부터 시작하도록
    public IEnumerator FlipCard()
    {
        Vector3 startSize = rect.localScale;
        rect.localScale *= 1.2f;
        HandleDraggable(false);

        float startRotation = slotCard.CardFace == CardFace.Upright ? 180 : 0;
        float targetRotation = startRotation + 180f;
        float time = 0;

        float duration = 0.2f;

        ResetCardPos();

        while(time < duration)
        {
            time += Time.deltaTime;
            float easeOB = MyEase.EaseOutBack(time/duration);


            float z = startRotation + (targetRotation - startRotation) * easeOB;
            rect.localEulerAngles = new Vector3(0, 0, z);
            yield return null;
        }
        UpdateCardFace(slotCard.CardFace);
        
        yield return new WaitForSeconds(0.2f);
        rect.localScale = startSize;

        HandleDraggable(true);
    }



    
    // 카드 위치 초기화
    public void ResetCardPos()
    {
        transform.SetParent(previousParent);
        rect.position = previousParent.GetComponent<RectTransform>().position;
    }
}
