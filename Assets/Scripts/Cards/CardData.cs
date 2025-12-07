using System;
using System.Collections.Generic;
using UnityEngine;

public enum CardRank
{
    Normal,
    Rare,
    Hero,
    Legend
}

public enum CardFace
{
    Upright,
    Reversed
}


// 카드의 속성
public enum CardElement
{
    None,
    Fire,
    Ice,
    Grass,
    Lightning

}

// 카드의 데이터
public class CardData : ScriptableObject
{
    public string CardID;

    public string Name;
    public CardRank Rank;
    public CardElement Element;
        
    // 효과 구조 클래스
    public EffectStructure Effects;

    // 카드의 스프라이트
    public Sprite CardSprite;


    //public int evolutionLevel = 1;
    //public bool isUpgraded = false;


}


// 전투에서 덱에 쓰이는 카드
[Serializable]
public class GameCard
{
    public CardData Card { get; private set; }                // 카드 정보   
    public Guid CardGuid { get; private set; }                // 카드 id

    public CardFace CardFace { get; private set; }              // 카드의 방향

    //public bool isUpgraded;             // 업그레이드 여부
    //public List<Modifier> modifiers;    // 적용된 변화(버프)들 

    public GameCard(CardData card)
    {
        Card = card;
        CardGuid = Guid.NewGuid();
    }
    public void SetRandomCardFace()
    {
        CardFace = (UnityEngine.Random.value < 0.5f) ? CardFace.Upright : CardFace.Reversed;
    }

    public void Flip()
    {
        CardFace = CardFace == CardFace.Upright ?
            CardFace.Reversed :
            CardFace.Upright;
    }
    //// 업그레이드는 현재는 사용안함
    //public void UpgradeCard()
    //{
    //    if (isUpgraded)
    //    {
    //        Debug.LogError("이 카드는 이미 업그레이드 되어있습니다");
    //        return;
    //    }

    //    isUpgraded = true;
    //}

}


