 using System;
using System.Collections;
using UnityEngine;
public enum TurnPhase
{
    PlayerStart,
    PlayerMain,
    PlayerEnd,
    EnemyStart,
    EnemyMain,
    EnemyEnd
}

// 불은 상대 턴 시작
// 빙결은 상대턴 시작(어는 것은 동시?)
// 감전은 공격과 동시
// 독은 상대턴 시작



// 내 턴 종료 시 : 턴 종료 트리거 뒷면 효과 발동
// 상대 턴 중 : 공격 받으면 소멸 트리거 발동


// 각 턴을 어떻게 간단하게 나타낼지 고려중. bool로 하려니 너무 많다
public class TurnManager : MonoBehaviour
{
    BattleProcessor Processor { get; set; }
    BattleCardController CardController { get; set; }
    //Enemy CurrEnemy { get; set; }

    private TurnPhase _phase;


    public event Action OnPlayerStart;
    public event Func<IEnumerator> OnPlayerEnd;

    public event Action<bool, int> OnBattleOver;

    public bool battleOver = false;

    [Header("Controllable")] public bool HasEndedEnemyTurn;


    public void InitializeTurnManager(BattleProcessor processor, BattleCardController cardController, Enemy currEnemy)
    {
        Processor = processor;
        CardController = cardController;
        //CurrEnemy = currEnemy;


    }


    public void StartBattle()
    {
        battleOver = false;
        Processor.turnCount = 0;
        CardController.ShuffleCard();   // 시작 시 셔플

        StartCoroutine(TurnLoop());
        
    }


    // 전투 동안에 지속되는 턴 루프
    IEnumerator TurnLoop()
    {

        while (!battleOver)
        {
            Processor.turnCount++;

            // -------플레이어 턴------
            StartPlayerTurn();
            yield return new WaitUntil(() => _phase == TurnPhase.PlayerEnd);
            
            yield return StartCoroutine(EndPlayerTurnCoroutine());

            if (battleOver) break;

            
            yield return new WaitForSeconds(1.2f);


            // ------적 턴------
            StartEnemyTurn();
            yield return new WaitUntil(() => _phase == TurnPhase.EnemyEnd);

            yield return new WaitForSeconds(1.2f);

            if (battleOver) break;
        }

    }

    // 내 턴 시작시
    private void StartPlayerTurn()
    {
        _phase = TurnPhase.PlayerStart;

        CardController.TryRerollCard(true);        // 턴 시작 시 패 리롤
        Processor.ShowPatternPreview();     // 적 공격 예상 표시

        OnPlayerStart?.Invoke();

        _phase = TurnPhase.PlayerMain;

    }

    // 내 턴 종료시
    // 상태이상 및 턴 종료 트리거 뒷면 카드 발동
    private IEnumerator EndPlayerTurnCoroutine()
    {
        if(OnPlayerEnd != null)
        {
            foreach (Func<IEnumerator> effect in OnPlayerEnd.GetInvocationList())
            {
                yield return StartCoroutine(effect());
            }
        }

        var enemy = MasterBattleManager.Instance.CurrEnemy;
        enemy.RefreshSEDic();
    }

    // 적 턴 시작시
    private void StartEnemyTurn()
    {
        _phase = TurnPhase.EnemyStart;

        StartCoroutine(EnemyTurnCoroutine());
    }



    // 적 턴, 적이 공격하는 코루틴
    public IEnumerator EnemyTurnCoroutine()
    {
        _phase = TurnPhase.EnemyMain;
        var enemy = MasterBattleManager.Instance.CurrEnemy;
        if(enemy.isFrozen)
        {
            Debug.Log("적은 얼어붙었다!"); 
            // 적이 얼어붙은 모습 표시
            yield return new WaitForSeconds(0.5f);

            _phase = TurnPhase.EnemyEnd;
            yield break;
        }
        var instances = enemy.PatternHandler.AttackInstances;

        for(int i=0; i<instances.Count;i++)
        {
            yield return StartCoroutine(Processor.ExecuteEnemyAttack(instances[i]));  
        }    
        _phase = TurnPhase.EnemyEnd;
    }




    // UI 턴 종료 버튼 용 함수
    public void OnPressEndButton()
    {
        if (_phase == TurnPhase.PlayerMain)
            _phase = TurnPhase.PlayerEnd;
    }


    // isPlayerDead 를 true로 할 시 플레이어 패배
    // 그 외엔 적 처치
    public void BattleOver(bool isPlayerDead = false)
    {
        Debug.Log("전투 종료");
        if (battleOver)
            return;

        battleOver = true;
        
        if(isPlayerDead)
        {
            OnBattleOver?.Invoke(false,0);
            Debug.Log("플레이어 패배");
        }
        else
        {
            int gold = MasterBattleManager.Instance.CurrEnemy.rewardGold;

            OnBattleOver?.Invoke(true,gold);
            Debug.Log("적 처치");
        }
    }

}






