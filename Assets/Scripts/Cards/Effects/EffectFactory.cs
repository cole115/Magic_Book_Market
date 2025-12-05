using System.Collections.Generic;
using UnityEngine;

// EffectExecutor에서 카드 효과에 대한 정보를 받으면 생성
public class EffectFactory
{
    private IEffect CreateEffect(EffectInfo info)
    {
        switch (info.Type)
        {
            case EffectType.DealDamage:
                return new Effect_DealDamage(info.Value);
            case EffectType.IncreaseAttack:
                return new Effect_IncreaseAttack(info.Value);
            case EffectType.InflictSE:
                return new Effect_InflictSE(info.Value);
            case EffectType.DrawCard:
                return new Effect_DrawCard(info.Value);
            case EffectType.AddReroll:
                return new Effect_AddReroll(info.Value);
            case EffectType.HealMiasma:
                return new Effect_HealMiasma(info.Value);
            case EffectType.RecovoerMana:
                return new Effect_RecoverMana(info.Value);

            default:
                return null;
        }


    }

    public List<IEffect> CreateEffects(List<EffectInfo> infos)
    {
        var list = new List<IEffect>();
        foreach(var info in infos)
            list.Add(CreateEffect(info));
        return list;

    }

}
