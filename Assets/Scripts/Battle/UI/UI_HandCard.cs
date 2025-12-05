using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 패의 카드의 ui를 관리할 클래스
public class UI_HandCard : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{



    public Image CardImage{get;private set;}

    Sprite placeHolder;
    Transform canvas;
    Transform previousParent;
    RectTransform rect;

    // 슬롯 번호는 0~3. 계산상의 편의를 위해
    public int SlotIndex { get; private set; }


    // 카드 사용 시도 이벤트
    // 카드 방향 및 마나 체크하고 가능 여부를 bool로 return
    public event Func<int, bool> OnTryUseCard;

    // 카드 플립 시도 이벤트
    // 마나 체크하고 가능 여부를 bool로 return
    public event Func<int, bool> OnTryFlipCard;

    private CardFace _cardFace;
    public bool canDrag = true;


    private void Awake()
    {
        CardImage = GetComponent<Image>();

        canvas = transform.root;
        rect = GetComponent<RectTransform>();
    }

   
    public void Init(int slotIndex, Sprite placeHolder)
    {
        SlotIndex = slotIndex;
        this.placeHolder = placeHolder;
    }



    // 플레이스 홀더 보여주기
    // 반투명하게 하니까 슬롯이랑 색이 겹쳐서 진해지므로 그냥 알파값 0으로
    public void ShowPlaceHolder()
    {
        if(placeHolder != null)
        {
            CardImage.sprite = placeHolder;

            Color c = CardImage.color;
            c.a = 0f;
            CardImage.color = c;
        }
        
    }



    // ui_cardview에서 스프라이트 가져오기
    // 카드 정보는 BattleUIPresenter가 갖고 있음
    // cardFace에 따라 canDrag와 카드 방향이 결정됨
    public void ShowCard(Sprite sprite, CardFace face)
    {
        CardImage.enabled = true;
        CardImage.sprite = sprite;

        Color c= CardImage.color;
        c.a = 1f;
        CardImage.color = c;


        HandleDraggable(true);
        
        _cardFace = face;
        UpdateCardFace(face);
    }


    private void UpdateCardFace(CardFace face)
    {
        if (face == CardFace.Upright)
        {
            rect.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            rect.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
    }


    // 카드 비움
    public void EmptyCard()
    {
        ResetCardPos(); 
        HandleDraggable(false);
        CardImage.sprite = placeHolder;
        rect.localScale = Vector3.one;  
        //image.enabled = false;
        //gameObject.SetActive(false);
    }

    // 드래그 가능 여부
    public void HandleDraggable(bool isAble)
    {
        canDrag = isAble;
        CardImage.raycastTarget = isAble;
    }


    #region OnDrag
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        previousParent = transform.parent;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        CardImage.raycastTarget = false;

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

        CardImage.raycastTarget = true;
    }

    #endregion

    // 카드를 쓸 수 있는지 체크
    public bool CanUseCard()
    {
        return OnTryUseCard?.Invoke(SlotIndex) ?? false;
    }

    public bool CanFlipCard()
    {
        return OnTryFlipCard?.Invoke(SlotIndex) ?? false;
    }

    // 카드 발동 시 실행. 카드 사용 시 나오는 연출만.
    //public IEnumerator ShowActivateCoroutine()
    //{

    //    //rect.localScale *= 2.0f;
    //    HandleDraggable(false);
    //    ResetCardPos();

    //    yield return new WaitForSeconds(1.0f);

        
    //    EmptyCard();
    //}


    // 패의 카드 뒤집을 시 실행
    // 카드 방향이 뒤집기 전에 이미 정해져 버리도록 만들어져 있으므로, 차라리 현재 방향을 참조해서 그 반대방향부터 시작하도록
    public IEnumerator FlipCard()
    {
        Vector3 startSize = rect.localScale;
        rect.localScale *= 1.2f;
        HandleDraggable(false);

        float startRotation = _cardFace == CardFace.Upright ? 180 : 0;
        float targetRotation = startRotation + 180f;
        float time = 0;

        float duration = 0.2f;

        ResetCardPos();

        while(time < duration)
        {
            time += Time.deltaTime;
            float easeOB = MyEase.EaseOutBack(time/duration);


            float z = startRotation + (targetRotation - startRotation) * easeOB;
            rect.transform.localEulerAngles = new Vector3(0, 0, z);
            yield return null;
        }
        UpdateCardFace(_cardFace);
        
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
