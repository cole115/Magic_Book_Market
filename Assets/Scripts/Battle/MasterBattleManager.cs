using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;


// 전투 씬 + 스테이지 돌 때 쓰는 매니저
public class MasterBattleManager : MonoBehaviour
{
    public static MasterBattleManager Instance {  get; private set; }
    public BattleCardController CardController { get; private set; }
    public BattleProcessor Processor { get; private set; }    
    public EffectExecutor EffectExecutor { get; private set; }

    public TurnManager TurnManager;
    public RunInfo runInfo;

    public Enemy CurrEnemy;

    
    [SerializeField] BattleUIPresenter uiPresenter;
    
    // 임시. 나중에 고침
    public BattleUIPresenter PublicUIPresenter => uiPresenter;

    //private Dictionary<string, AsyncOperationHandle<Sprite>> loadedSprites = new();


    private async void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        MakeRunInfoIfNull();
        await InitializeBattle();


        //EffectExecutor = new EffectExecutor();

    }

    private void MakeRunInfoIfNull()
    {
        if(runInfo == null)
        {
            runInfo = new RunInfo();
        }
    }



    private async Task InitializeBattle()
    {
        

        await SetCurrEnemy(runInfo.EnemyName);
        
        await CreateSampleDeck();
        CardController = new BattleCardController(runInfo.DeckList);
        Debug.Log($"덱 장수 : {runInfo.DeckList.Count}");

        Processor = new BattleProcessor(CardController, runInfo, CurrEnemy);
        TurnManager.InitializeTurnManager(Processor, CardController,CurrEnemy);

        EffectExecutor = new(CurrEnemy, Processor);
        StartCoroutine(EffectExecutor.ProcessQueueLoop());

        uiPresenter.InitUIPresenter();
        TurnManager.StartBattle();
    }


    // 현재 적 & 패턴 로드
    private async Task SetCurrEnemy(string monsterID)
    {
        var handleEnemy = Addressables.LoadAssetAsync<EnemySO>(monsterID);
        await handleEnemy.Task;

        CurrEnemy.SetEnemy(handleEnemy.Result);

        var handlePatterns = Addressables.LoadAssetsAsync<Enemy_Pattern>("Patterns",null);
        await handlePatterns.Task;

        List<Enemy_Pattern> allPatterns = handlePatterns.Result.ToList();
        
        var filteredPatterns = allPatterns.Where(p => p.MonsterID == monsterID).ToList();

        CurrEnemy.SetPatternList(filteredPatterns);

        Addressables.Release(handleEnemy);
        Addressables.Release(handlePatterns);
    }




    // 모든 카드 데이터는 스테이지가 시작할 때 로드
    // 임시로 여기서 로드함
    
    private async Task CreateSampleDeck()
    {
        // 카드 데이터 전부 로드
        await CardDatabase.LoadAllCards();

        runInfo.InitDeck();

        runInfo.ClearDeck();
        // 카드 랜덤 30장 생성
        for(int i=0;i<30;i++)
        {
            var gameCard = CardDatabase.GetRandomCard();
            runInfo.AddCardToDeck(gameCard);

        }
        Debug.Log($"카드 장수 : {runInfo.DeckList.Count}");
    }





    // 캐싱 하는 것이 더 낫다는 판단 하에 Release는 안함
    //private void ReleaseLoadedSprites()
    //{
    //    foreach(var kvp in loadedSprites)
    //    {
    //        Addressables.Release(kvp.Value);
    //    }
    //    loadedSprites.Clear();
    //}


}




