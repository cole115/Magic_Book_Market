using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 카드 효과 큐 처리
// 정방향 역방향의 효과 처리 구별
// 상태이상 처리
public class EffectExecutor
{
    // 큐에 넣을 구조체
    private struct EffectTask
    {
        public IEffect Effect { get; private set; }
        public GameCard SourceCard { get; private set; }

        public EffectTask(IEffect effect, GameCard sourceCard)
        {
            Effect = effect;
            SourceCard = sourceCard;
        }

    }
    public BattleProcessor processor { get; private set; }
    public Enemy enemy { get; private set; }


    // 카드 큐
    Queue<EffectTask> effectQueue = new();
    EffectFactory effectFactory = new();

    private bool _isProcessing = false;






    public EffectExecutor(Enemy enemy, BattleProcessor processor)
    {
        this.enemy = enemy;
        this.processor = processor;

        MasterBattleManager.Instance.CardController.OnCardUsed += EnqueueUprEffects;
        MasterBattleManager.Instance.TurnManager.OnPlayerEnd += OnTurnEnd;
    }


    public IEnumerator ProcessQueueLoop()
    {
        // 대미지가 지금 너무 쎄서 테스트가 안됨...
        //while(!MasterBattleManager.Instance.TurnManager.battleOver)
        while (true)
        {
            if (effectQueue.Count > 0 && !_isProcessing)
            {

                yield return ExecuteEffects();
            }
            else
            {
                yield return null;
            }

        }
    }

    private IEnumerator ExecuteEffects()
    {
        _isProcessing = true;

        while (effectQueue.Count > 0)
        {
            var task = effectQueue.Dequeue();

            task.Effect.Execute(processor, task.SourceCard);
            yield return new WaitForSeconds(0.3f);

            if (task.Effect.EffectType == EffectType.DealDamage)
            {
                if (enemy.inflictedSE.TryGetValue(StatusEffectType.Shock, out var shock))
                {
                    shock.OnDealDamage(this);
                    yield return new WaitForSeconds(0.3f);
                }
            }

        }
        _isProcessing = false;

    }


    // 정방향 효과 enqueue
    public void EnqueueUprEffects(GameCard card)
    {
        if (card.CardFace == CardFace.Reversed)
        {
            Debug.LogWarning("정방향 카드만 처리할 수 있습니다");
            return;
        }

        Debug.Log($"정방향 카드 발동 : {card.Card.Name}");
        int attackCount = card.Card.Effects.AttackCount;

        for (int i = 0; i < attackCount; i++)
        {
            // 역방향 효과(지속) enqueue
            EnqueueRevEffects(RevEffectTrigger.Continuous);


            List<IEffect> effects = effectFactory.CreateEffects(card.Card.Effects.UprEffectInfos);


            foreach (IEffect effect in effects)
            {
                // 정방향 효과 Enqueue
                effectQueue.Enqueue(new EffectTask(effect, card));

            }

        }


    }




    // 역방향 효과 enqueue
    // 
    private void EnqueueRevEffects(RevEffectTrigger trigger)
    {
        var revCards = MasterBattleManager.Instance.CardController.HandCards.Hand_GetAllRevEffects(trigger);


        foreach (var c in revCards)
        {
            Debug.Log($"역방향 카드 발동 : {c.Card.Name}");
            List<IEffect> effects = effectFactory.CreateEffects(c.Card.Effects.RevEffectInfos);
            foreach (var effect in effects)
            {
                effectQueue.Enqueue(new EffectTask(effect, c));
            }
        }

    }




    // ----------상태이상------------


    // 턴 종료 시 발동하는 상태이상
    public IEnumerator ExecuteSEOnTurnEnd()
    {
        foreach (IStatusEffect se in enemy.inflictedSE.Values)
        {
            if (se.Type == StatusEffectType.Shock)
                continue;

            se.OnTurnEnd(this);
            yield return new WaitForSeconds(0.3f);

        }

    }

    //---------- 외부에서 실행할 코루틴 -------------

    // 턴 종료 시 발생할 효과들 을 return
    public IEnumerator OnTurnEnd()
    {
        EnqueueRevEffects(RevEffectTrigger.TurnEnd);
        yield return ExecuteEffects();
        yield return ExecuteSEOnTurnEnd();
    }
}
