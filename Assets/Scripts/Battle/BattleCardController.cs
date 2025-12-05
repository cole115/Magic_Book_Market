using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 전투 중 카드 조작 전반
public class BattleCardController
{
    public List<GameCard> MainDeck = new();
    public List<GameCard> RecycleDeck = new();

    public HandCards HandCards = new();

   
    public Action<GameCard> OnCardUsed; // 카드 사용 이벤트
    public event Func<int,bool> OnManaSpent; // 마나 소비 이벤트
    public event Func<bool> OnRerollSpent; // 리롤 소비 이벤트

    public const int FLIP_COST = 1;



    public BattleCardController(List<GameCard> originalDeck)
    {
        MainDeck = new List<GameCard>(originalDeck);
    }



    // 카드를 덱에서 드로우했을 때
    // recycle덱 기능은 아직
    public void DrawCard()
    {
        if (MainDeck.Count == 0)
        {
            RecycleToMain();
            

        }
        if (MainDeck.Count == 0)
        {
            Debug.Log("드로우할 카드가 없습니다");
            return;
        }


        if (HandCards.HandSlots.All(x => x != null))
        {
            Debug.Log("패에 자리가 없습니다");
            return;
        }


        GameCard c = MainDeck[0];
        MainDeck.RemoveAt(0);

        // 드로우 시 카드 방향 랜덤 설정
        c.SetRandomCardFace();

        HandCards.Hand_GetCard(c);
    }


    // 카드 사용. 쓴 카드는 리사이클 덱으로
    public bool TryUseCard(int index)
    {
        if (!CanUseCard(index))
            return false;

        GameCard used = HandCards.Hand_UseCard(index);        
        RecycleDeck.Add(used);
        
        OnCardUsed?.Invoke(used);

        for (int i = 0; i < HandCards.HandSlots.Length; i++)
        {
            if (HandCards.HandSlots[i] != null)
                Debug.Log($"슬롯 {i + 1} : {HandCards.HandSlots[i].Card.CardID}");

        }
        return true;
    }

    private bool CanUseCard(int index)
    {
        var card = HandCards.HandSlots[index];
        if(card == null || card.CardFace != CardFace.Upright)
        {
            Debug.Log("카드 사용 실패");
            return false;

        }
            

        bool hasMana = OnManaSpent?.Invoke(card.Card.Effects.Cost) ?? false;
        return hasMana;
    }

    // 패를 리롤
    // 턴 시작에선 리롤 소비 안함
    public bool TryRerollCard(bool isTurnStart = false)
    {
        if(!isTurnStart)
        {
            bool hasReroll = OnRerollSpent?.Invoke() ?? false;
            if (!hasReroll)
                return false;
        }

        Debug.Log("패 리롤");
        List<GameCard> rerolled = HandCards.Hand_Empty();
        RecycleDeck.AddRange(rerolled);

        for (int i = 0; i < HandCards.HAND_NUMBER; i++)
        {
            DrawCard();
        }

        return true;
    }

    public bool TryFlipCard(int index)
    {
        var card = HandCards.HandSlots[index];
        bool hasMana = OnManaSpent?.Invoke(FLIP_COST) ?? false;

        if (card == null || !hasMana)
            return false;
        Debug.Log("패 플립");
        HandCards.Hand_Flip(index);

        return true;       
    }



    // 덱을 셔플
    public void ShuffleCard()
    {
        Debug.Log("덱 셔플");
        int n = MainDeck.Count;

        for (int i = MainDeck.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);

            GameCard temp = MainDeck[i];
            MainDeck[i] = MainDeck[rnd];
            MainDeck[rnd] = temp;

        }
    }


    public void RecycleToMain()
    {
        if (RecycleDeck.Count == 0)
        {
            Debug.Log("리사이클 덱이 비어있습니다");
            return;
        }

        MainDeck.AddRange(RecycleDeck);
        RecycleDeck.Clear();

        ShuffleCard();

        Debug.Log("리사이클 완료");

    }



    // guid를 사용한 삭제는 현재는 필요없음

    //public Card DeleteCard(GameCard gameCard)
    //{
    //    Debug.Log($"{gameCard.card.CardName} 카드 삭제");
    //    Guid id = gameCard.cardGuid;

    //    for (int i = mainDeck.Count - 1; i >= 0; i--)
    //    {
    //        if (mainDeck[i].cardGuid == id)
    //        {
    //            mainDeck.RemoveAt(i);
    //            break;
    //        }

    //    }

    //    return gameCard.card;
    //}

    public void DeleteCardInDeck(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(MainDeck.Count == 0 )
            {
                RecycleToMain();
            }
            
            if(MainDeck.Count ==0)
            {
                Debug.Log("덱이 남아있지 않습니다");

                MasterBattleManager.Instance.TurnManager.BattleOver(isPlayerDead: true);
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, MainDeck.Count);
            GameCard deletedCard = MainDeck[randomIndex];
            MainDeck.RemoveAt(randomIndex);

            Debug.Log($"{deletedCard.Card.Name}이 덱에서 삭제됨");
            // 카드 삭제 연출 삽입
        }

    }

    

}






// 패에서 사용가능한 기능
public class HandCards
{
    public const int HAND_NUMBER = 4;

    public GameCard[] HandSlots { get; private set; } = new GameCard[HAND_NUMBER];


    // 패의 구성이 바뀌었을 때 실행하는 action
    public event Action OnHandCardChanged;

    // 패의 카드가 삭제되었을 때 실행하는 이벤트. 카드 슬롯 값 받음
    public event Action<int> OnHandCardBreak;

    // 카드가 없는 첫번째 슬롯 번호
    private int Hand_FindFirstEmptySlot()
    {
        return Array.FindIndex(HandSlots, c => c == null);
    }



    // 패에 카드를 추가
    public void Hand_GetCard(GameCard card)
    {
        int emptySlot = Hand_FindFirstEmptySlot();

        if (emptySlot != -1)
        {
            HandSlots[emptySlot] = card;
        }
        else
        {
            Debug.LogError("패에 자리가 없습니다");
        }


        OnHandCardChanged?.Invoke();
    }


    // 패의 카드 사용
    public GameCard Hand_UseCard(int num)
    {
        if (HandSlots[num] == null)
        {
            Debug.LogError($"{num} 자리에 카드가 없습니다");
            return null;
        }

        if (HandSlots[num].CardFace == CardFace.Reversed)
        {
            Debug.LogWarning("역방향 카드는 사용할 수 없습니다");
            return null;
        }

        GameCard removed = HandSlots[num];
        HandSlots[num] = null;


        OnHandCardChanged?.Invoke();
        return removed;
    }


    // 패를 비움
    public List<GameCard> Hand_Empty()
    {
        List<GameCard> trash = new();
        for (int i = 0; i < HandSlots.Length; i++)
        {
            if (HandSlots[i] != null)
            {
                trash.Add(HandSlots[i]);
                HandSlots[i] = null;
            }


        }

        return trash;
    }


    // 패에 있는 카드를 삭제
    public void Hand_DeleteCardInHand(int num)
    {
        if (num < 0 || num >= HAND_NUMBER)
        {
            Debug.Log($"잘못된 인덱스 : {num}");
            return;
        }

        GameCard deletedCard = HandSlots[num];
        if (deletedCard == null)
        {
            Debug.Log($"{num+1}번 슬롯에 카드가 없습니다");
            return;
        }

        OnHandCardBreak?.Invoke(num);
        HandSlots[num] = null;

        Debug.Log($"{deletedCard.Card.Name}이(가) 패에서 삭제됨 ");
        OnHandCardChanged?.Invoke();
        

    }


    // 패에 있는 카드 뒤집기
    public void Hand_Flip(int num)
    {
        HandSlots[num].Flip();
        OnHandCardChanged?.Invoke();

    }




    // 패의 역방향 카드들을 return
    public List<GameCard> Hand_GetAllRevEffects(RevEffectTrigger triggerType)
    {
        return HandSlots.
            Where(card => card!=null &&
                card.CardFace == CardFace.Reversed &&
                card.Card.Effects.Trigger == triggerType)
                .ToList();

    }



}

