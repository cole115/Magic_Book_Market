using System; 
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public string enemyName;
    public int enemyMaxHealth;

    private int _enemyCurrHealth;
    public int EnemyCurrHealth
    {
        get => _enemyCurrHealth;
        set
        {
            _enemyCurrHealth = Mathf.Max(value, 0);
        }
    }

    public int enemyAttack;
    public int enemyDefense;

    public int rewardGold;

    [Header("Road Of Sin")]
    [SerializeField] SpriteRenderer background;
    [SerializeField] Sprite[] enemySprites = new Sprite[3];
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Sprite[] backgroundSprites = new Sprite[3];




    public Enemy_PatternHandler PatternHandler { get; private set; }

    // 상태이상 목록
    public Dictionary<StatusEffectType, IStatusEffect> inflictedSE = new();
    public bool isFrozen = false;   // 빙결에 의한 공격 차단

    public event Action<int,int> OnEnemyHealthChanged;
    public event Action<Dictionary<StatusEffectType, int>> OnSERefreshed;       // 현재 상태이상 목록 갱신
    public event Action<StatusEffectType, int> OnEnemySEActivated;  // 발동한 상태이상 대미지

    
    public void SetEnemy(EnemySO enemyData)
    {
        enemyName = enemyData.name;
        enemyMaxHealth = enemyData.Hp;
        EnemyCurrHealth = enemyMaxHealth;
        enemyAttack = enemyData.Atk ;
        enemyDefense = enemyData.Def;

        rewardGold = enemyData.GetGold;

        OnEnemyHealthChanged?.Invoke(EnemyCurrHealth,enemyMaxHealth);

        SetEnemySprite(enemyName);
    }

    private void SetEnemySprite(string name)
    {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        hpText.text = enemyMaxHealth.ToString();

        int index = name switch
        {
            "M1001" => 0,
            "M1010" => 1,
            "M1019" => 2,
            _ => throw new Exception("잘못된 적 코드입니다")
        };

        renderer.sprite = enemySprites[index];
        background.sprite = backgroundSprites[index];

    }




    public void SetPatternList(List<Enemy_Pattern> patterns)
    {
        PatternHandler = new Enemy_PatternHandler(patterns);
    }


    // 방어력만큼 받는 대미지 감소. 최소 1만큼
    public void TakeCardDamage(int amount)
    {
        int damageTaken = Mathf.Max(amount - enemyDefense, 1);
        EnemyCurrHealth -= damageTaken;

        OnEnemyHealthChanged?.Invoke(EnemyCurrHealth, enemyMaxHealth);

        if (EnemyCurrHealth <=0)
        {
            MasterBattleManager.Instance.TurnManager.BattleOver();
        }
        Debug.Log($"적 대미지. 현재 체력 : {EnemyCurrHealth}");
        
    }

    // 상태이상 대미지
    public void TakeSEDamage(StatusEffectType type, int amount)
    {
        //Debug.Log($"적 체력 {EnemyCurrHealth},상태이상 대미지 {amount}");
        EnemyCurrHealth -= amount;
        OnEnemySEActivated?.Invoke(type, amount);        
        OnEnemyHealthChanged?.Invoke(EnemyCurrHealth, enemyMaxHealth);  

        // 체력이 0 이하 시 전투 종료
        if (EnemyCurrHealth <= 0)
        {
            MasterBattleManager.Instance.TurnManager.BattleOver();  
        }

       
    }

    // 상태이상 대미지
    public void InflictSE(StatusEffectType seType, int amount)
    {
        // 이미 있는 상태이상이면 스택 증가
        if (inflictedSE.TryGetValue(seType, out IStatusEffect existingSE))
        {
            existingSE.AddStack(amount);
        }
        else
        {
            // 없으면 추가
            IStatusEffect newEffect = seType switch
            {
                StatusEffectType.Burn => new SE_Burn(),
                StatusEffectType.Freeze => new SE_Freeze(),
                StatusEffectType.Poison => new SE_Poison(),
                StatusEffectType.Shock => new SE_Shock(),
                _ => null
            };

            newEffect.AddStack(amount);
            inflictedSE.Add(seType, newEffect);
        }
        RefreshSEDic();
    }


    // 0스택이 된 상태이상 정리
    // 상태이상 UI 갱신
    public void RefreshSEDic()
    {
        foreach (var key in inflictedSE
            .Where(p => p.Value.Stack <= 0)  // 스택이 0 이하인 효과
            .Select(p => p.Key)
            .ToList())
        {
            inflictedSE.Remove(key);
        }

        var seDic = inflictedSE.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Stack
            );

        OnSERefreshed?.Invoke(seDic);
    }

}



// 적 패턴 핸들러
public class Enemy_PatternHandler
{
    private Enemy_Pattern nextPattern;
    private List<Enemy_Pattern> patternList;

    public List<AttackInstance> AttackInstances { get; private set; }

    public Enemy_PatternHandler(List<Enemy_Pattern> patternList)
    {

        this.patternList = patternList;
    }



    // 공격 패턴 랜덤 설정
    private Enemy_Pattern GetRandomPattern()
    {
        //float totalChance = 0f;
        //foreach (var p in patternList)
        //    totalChance += p.UseChance;

        float totalChance = patternList.Sum(p => p.UseChance);


        if (totalChance != 100)
        {
            Debug.LogError("패턴의 확률의 합이 100%가 아닙니다");
            return null;
        }

        float roll = UnityEngine.Random.Range(0, totalChance);
        float cumulative = 0f;

        foreach(var p in patternList)
        {
            cumulative += p.UseChance;
            if (roll <= cumulative)
                return p;
        }

        return patternList[^1];

    }

    // 다음에 적이 쓸 패턴 정보
    public List<AttackInstance> PrepareNextPattern()
    {
        nextPattern = GetRandomPattern();
        AttackInstances = nextPattern.GetAttackInstances();

        return AttackInstances;
    }

}
