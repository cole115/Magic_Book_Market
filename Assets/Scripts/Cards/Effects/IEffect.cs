using System;
using System.Collections.Generic;
using UnityEngine;

// 효과 타입
public enum EffectType
{
    DealDamage,    
    //AddScaling,
    IncreaseAttack, 
    InflictSE,
    DrawCard,
    AddReroll,
    HealMiasma,
    RecovoerMana
}
// 역방향 발동 트리거
public enum RevEffectTrigger
{
    //Vanish,       소멸은 일단 제외
    Continuous,
    TurnEnd

}

[Serializable]
public struct EffectInfo
{
    public EffectType Type;
    public float Value;
}



// 카드에 들어갈 효과구조
[Serializable]
public class EffectStructure
{
    // 정방향
    public int Cost;                                // 코스트
    public List<EffectInfo> UprEffectInfos = new(); // 정방향 효과 정보
    public string UprDescription;                   // 효과설명
    public int AttackCount;                         // 정방향 효과 발동 횟수


    // 역방향
    public RevEffectTrigger Trigger;                // 역방향 효과 트리거
    public List<EffectInfo> RevEffectInfos = new(); // 역방향 효과 정보
    public string RevDescription;                   // 효과 설명


}

public interface IEffect 
{
    EffectType EffectType { get; }
    float amount { get; }
    void Execute(BattleProcessor processor, GameCard card);
    
}


// 대미지를 주는 효과.
// 정/역 전부 있음
public class Effect_DealDamage : IEffect
{
    public EffectType EffectType => EffectType.DealDamage;

    public float amount { get; }

    public Effect_DealDamage(float amount)
    {
        this.amount= amount;
    }



    public void Execute(BattleProcessor processor, GameCard card)
    {
        int _baseDamage = processor.playerAttack;
        Debug.Log($"플레이어 공격력 : {_baseDamage}");
        processor.DealDamage((int)(_baseDamage * amount),card.Card.Element);
    }




}


// 패의 카드의 계수를 증가시키는 효과
// 적용 대상은 오직 공격(DealDamage)계열에만
// 역방향에만 존재
// 현재는 효과에 없음
//public class Effect_AddScaling : Effect
//{
//    public override EffectType effectType => EffectType.AddScaling;

//    public Effect_AddScaling(float _amount)
//    {
//        this._amount = _amount;
//    }
//    public override void Execute(BattleProcessor processor, GameCard card)
//    {
//        processor.AddScaling(_amount);
//    }
//}

// 공격력 업
// 역방향 상시에만 존재

public class Effect_IncreaseAttack : IEffect
{
    public EffectType EffectType => EffectType.IncreaseAttack;

    public float amount { get; }
    public Effect_IncreaseAttack(float amount)
    {
        this.amount = amount;
    }


    public void Execute(BattleProcessor processor, GameCard card)
    {
        processor.IncreaseAttack((int)amount);
    }


}

// 적에게 상태이상 부여
public class Effect_InflictSE : IEffect
{
    public EffectType EffectType => EffectType.InflictSE;

    public float amount { get; }
    public Effect_InflictSE(float amount)
    {
        this.amount = amount;
    }


    public void Execute(BattleProcessor processor, GameCard card)
    {
        CardElement element = card.Card.Element;

        StatusEffectType se = element switch
        {
            CardElement.Fire => StatusEffectType.Burn,
            CardElement.Ice => StatusEffectType.Freeze,
            CardElement.Grass => StatusEffectType.Poison,
            CardElement.Lightning => StatusEffectType.Shock,            
            _ => throw new ArgumentOutOfRangeException(nameof(element), $"상태이상을 부여할 수 없는 속성입니다 : {element}")

        };


        processor.InflictSE(se, (int)amount, element);
    }
}


public class Effect_DrawCard : IEffect
{
    public EffectType EffectType => EffectType.DrawCard;

    public float amount { get; }

    public Effect_DrawCard(float amount)
    {
        this.amount = amount;
    }

    public void Execute(BattleProcessor processor, GameCard card)
    {
        processor.DrawCard((int)amount);
    }

}

public class Effect_AddReroll : IEffect
{
    public EffectType EffectType => EffectType.AddReroll;

    public float amount { get; }

    public Effect_AddReroll(float amount)
    {
        this.amount = amount;
    }

    public void Execute(BattleProcessor processor, GameCard card)
    {
        processor.AddReroll((int)amount);
    }
}


public class Effect_HealMiasma : IEffect
{
    public EffectType EffectType => EffectType.HealMiasma;

    public float amount { get; }
    public Effect_HealMiasma(float amount)
    {
        this.amount = amount;
    }


    public void Execute(BattleProcessor processor, GameCard card)
    {
        processor.HealMiasma((int)amount);
    }
}

public class Effect_RecoverMana : IEffect
{
    public EffectType EffectType => EffectType.RecovoerMana;

    public float amount { get; }
    public Effect_RecoverMana(float amount)
    {
        this.amount= amount;
    }


    public void Execute(BattleProcessor processor, GameCard card)
    {
        processor.RecoverMana((int)amount);
    }
}


