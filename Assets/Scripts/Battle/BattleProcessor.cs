using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 전투 정보 및 처리
public class BattleProcessor
{
    private BattleCardController _battleCardController;
    public int playerAttack { get; private set; }

    // 오염도(무조건 0보다 큼)
    private int _currMiasma;
    public int CurrMiasma
    {
        get => _currMiasma;
        set
        {
            _currMiasma = Mathf.Max(value,0);
        }
    }


    // 마나(0보다 이상 _maxMana 이하)
    private int _maxMana;

    private int _currMana;
    public int CurrMana
    {
        get => _currMana;
        set
        {
            _currMana = Mathf.Clamp(value, 0, _maxMana);
        }
    }

   
    
    private int _rerollCount;   // 리롤 카운트

    public int turnCount;     // 턴 카운트 

    // 적 스펙(참조)-----------
    private Enemy _currEnemy;

    // 나중에 뺌
    public int enemyMaxHp { get; set; }
    int _enemyCurrHp;



    public event Action<List<AttackInstance>> OnEnemyAttackPreview; // 적 공격 예고
    public event Action<int> OnEnemyTryAttack;                      // 적 공격 시도(UI용)
    public event Action<int> OnEnemyAttackEffect;                   // 적 공격 이펙트
    public event Action<int> OnAttackEvaded;                        // 적 공격 회피
    public event Action<int, int> OnAttackCorrupted;                // 적 공격으로 오염도 증가
    public event Action OnEnemyPatternCleared;                      // 적 공격 초기화


    public event Action<int,int> OnMiasmaChanged;
    public event Action<int, int> OnManaChanged;
    public event Action<int> OnRerollChanged;
    public event Action<EffectType,int,CardElement?> OnEffectActivated;

    public event Action OnMiasmaOverflow;


    // BattleProcessor 및 전투 상황 초기화
    public BattleProcessor(BattleCardController cardController, RunInfo runInfo, Enemy enemy)
    {
        _battleCardController = cardController;
        _currEnemy = enemy; 

        playerAttack = 100;
        CurrMiasma = runInfo.Miasma;
        turnCount = 0;
        _maxMana = 20;

        MasterBattleManager.Instance.TurnManager.OnPlayerStart += InvokeOnTurnStart;

        MasterBattleManager.Instance.CardController.OnManaSpent += TrySpendMana;
        MasterBattleManager.Instance.CardController.OnRerollSpent += TrySpendReroll;

    }

    // 턴이 시작됐을 시 실행될 함수
    // MasterBattleManager에서 구독
    public void InvokeOnTurnStart()
    {
        turnCount++;
        _rerollCount = 1;
        CurrMana = _maxMana;
        OnMiasmaChanged?.Invoke(CurrMiasma, 100);
        OnRerollChanged?.Invoke(_rerollCount);

        Debug.Log($"현재 마나 : {CurrMana}");
    }



    // 효과 처리용 함수들
    #region Card Effects
    
    // 적에게 대미지
    public void DealDamage(int damage, CardElement element)
    {
        _currEnemy.TakeCardDamage(damage);
        OnEffectActivated?.Invoke(EffectType.DealDamage, damage,element);

    }

    // 패의 공격 카드의 계수 추가(현재는 효과에 없음)
    //public void AddScaling(float amount)
    //{
    //    var handArray = _battleCardController.handCards.handSlots;

    //    for (int i = 0; i < handArray.Length; i++)
    //    {
    //        if (handArray[i] !=null)
    //        {

    //            var uprEffect = handArray[i].card.Effects.UprEffects.
    //                Find(e => e.effectType == EffectType.DealDamage);

    //            if(uprEffect is Effect_DealDamage dmgEffect)
    //            {
    //                dmgEffect.AddScaling(amount);
    //            }
    //        }

    //    }

    //}

    // 공격력 강화
    public void IncreaseAttack(int amount)
    {
        playerAttack += amount;
        OnEffectActivated?.Invoke(EffectType.IncreaseAttack, amount, null);
    }

    // 상태이상 부여
    public void InflictSE(StatusEffectType type, int amount,CardElement element)
    {
        _currEnemy.InflictSE(type, amount);
        OnEffectActivated?.Invoke(EffectType.InflictSE, amount, element);
    }

    // 드로우
    public void DrawCard(int amount)
    {
        _battleCardController.DrawCard();
        OnEffectActivated?.Invoke(EffectType.DrawCard, amount, null);
    }

    // 리롤 추가
    public void AddReroll(int amount)
    {
        _rerollCount += amount;
        OnRerollChanged?.Invoke(_rerollCount);
        OnEffectActivated?.Invoke(EffectType.AddReroll, amount,null);
        

    }

    // 오염도 회복(일부 효과에 한해 오염 증가)
    // 오염 증가 효과는 현재 없음?
    public void HealMiasma(int amount)
    {
        if (amount > 0)
            DecreaseMiasma(amount);
        if(amount <0)
            IncreaseMiasma(amount);
    }
    
    // 마나 회복(카드 효과에 의한 것만 처리)
    public void RecoverMana(int amount)
    {
        CurrMana += amount;
        OnEffectActivated?.Invoke(EffectType.RecovoerMana, amount, null);
        OnManaChanged?.Invoke(CurrMana, _maxMana);
    }
    #endregion



    // 오염도 감소, 주로 카드 효과를 통한 회복
    public void DecreaseMiasma(int amount)
    {
        CurrMiasma -= amount;
        OnMiasmaChanged?.Invoke(CurrMiasma,100);
        OnEffectActivated?.Invoke(EffectType.HealMiasma, amount, null);
    }

    
    // 카드 사용에 의한 마나 소비
    public bool TrySpendMana(int amount)
    {
        if (CurrMana < amount)
        {
            Debug.Log($"마나 부족 : 현재 마나 {CurrMana}, 카드 마나 {amount}"); 
            return false;
        }
           
        CurrMana-= amount;
        Debug.Log($"현재 마나 : {CurrMana}, 소비 마나 : {amount}");

        OnManaChanged?.Invoke( CurrMana,_maxMana );
        Debug.Log($"현재 마나 {CurrMana}");
        return true;
    }

    public bool TrySpendReroll()
    {
        if(_rerollCount <=0)
            return false;

        _rerollCount--;
        OnRerollChanged?.Invoke(_rerollCount);
        return true;
    }


    // 오염도 증가, 주로 적의 공격을 통해 적용
    public void IncreaseMiasma(int amount)
    {
        int total = CurrMiasma + amount;
        int overflow = total / 100;
        CurrMiasma = total % 100;
        if (overflow > 0)
        {
            _battleCardController.DeleteCardInDeck(overflow);
            OnMiasmaOverflow?.Invoke();
        }
            

        Debug.Log($"_______현재 오염도{CurrMiasma}");
        OnMiasmaChanged?.Invoke(CurrMiasma, 100);
    }

    // 내 턴에 미리 적의 패턴 보여줌
    public void ShowPatternPreview()
    {
        OnEnemyAttackPreview?.Invoke(_currEnemy.PatternHandler.PrepareNextPattern());   
    
    }

    // 패턴  UI 초기화
    public void ClearPreview()
    {
        OnEnemyPatternCleared?.Invoke();
    }

    // 공격 방향에 카드가 있을 시 : 해당 카드 삭제
    // 공격 방향에 카드가 없을 시 : 회피 계산 후 실패 시 오염도 증가
    public IEnumerator ExecuteEnemyAttack(AttackInstance instance)
    {
        Debug.Log($"공격 방향 : {instance.Direction + 1}, 명중률 : {instance.HitChance}");       
        // 1) 적 공격 시도 연출
        OnEnemyTryAttack?.Invoke(instance.Direction);
        yield return new WaitForSeconds(0.6f);

        // 2) 적 공격 이펙트 연출
        OnEnemyAttackEffect?.Invoke(instance.Direction);
        yield return new WaitForSeconds(0.3f);

        EvadeResult result = EvadeCalculator.TryEvade(instance, MasterBattleManager.Instance.CardController.HandCards.HandSlots);


        // 3) 적 공격 처리 및 결과 연출(오염도 증가, 회피). 카드 파괴 연출은 따로

        if (result.IsCardDestroyed)      // 공격 받았을 때 & 카드가 깨졌을 때
        {
            MasterBattleManager.Instance.CardController.HandCards.Hand_DeleteCardInHand(instance.Direction);
            
            Debug.Log($"{instance.Direction + 1}슬롯 피격. 카드 파괴");
        }
        else if (!result.IsEvaded)      // 공격 받았을 때 & 카드가 없어 오염도가 증가했을 때
        {
            int damage = (int)(_currEnemy.enemyAttack * instance.Multiplier); 
            IncreaseMiasma(damage);
            OnAttackCorrupted?.Invoke(instance.Direction, damage);

            Debug.Log($"{instance.Direction + 1}슬롯 피격. 오염도 {damage} 상승");
        }
        else // 공격을 회피했을 때
        {
            OnAttackEvaded?.Invoke(instance.Direction);
            Debug.Log("공격 회피");
        }

        // 다음 공격 까지 잠깐 멈춤
        yield return new WaitForSeconds(0.5f);
    }
  
}

