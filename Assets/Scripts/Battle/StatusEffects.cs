using UnityEngine;

// 상태이상
// 방어력 무시 대미지
// 효과 별 발동 조건, 감쇠 여부, 대미지가 다름
public enum StatusEffectType
{
    Burn,
    Freeze,
    Poison,
    Shock
}


public interface IStatusEffect
{ 
    StatusEffectType Type { get; }
    int Stack { get; }

    void AddStack(int amount);

    void OnTurnEnd(EffectExecutor executor);
    void OnDealDamage(EffectExecutor executor);

}

public abstract class StatusEffectBase : IStatusEffect
{
    public abstract StatusEffectType Type { get; }
    public int Stack { get; protected set; }

    public virtual void AddStack(int amount)
    {
        Stack += amount;
    }
    public virtual void OnDealDamage(EffectExecutor executor) { }
    public virtual void OnTurnEnd(EffectExecutor executor) { }
}


// 상태이상 : 화상
// 내 턴 종료 시
// (공격력/4) * 스택 대미지
// 화상 발동 후 스택 소멸

public class SE_Burn : StatusEffectBase
{
    public override StatusEffectType Type => StatusEffectType.Burn;

    public override void OnTurnEnd(EffectExecutor executor)
    {
        
        int damage = (executor.processor.playerAttack/4) * Stack;        
        executor.enemy.TakeSEDamage(Type, damage);
        Stack = 0;
        Debug.Log($"화염/{Stack}스택 : {damage}");

    }
}

// 상태이상 : 빙결
// 내 턴 종료 시
// 스택 50 이상 시 적 공격 취소
// 그 다음 내 턴 종료 시
// (스택-50) * 100 대미지 후 스택 초기화
public class SE_Freeze : StatusEffectBase
{
    public override StatusEffectType Type => StatusEffectType.Freeze;

    public override void OnTurnEnd(EffectExecutor executor)
    {
        var enemy = executor.enemy;

        if(!enemy.isFrozen)
        {
            if(Stack>=50)
            {
                enemy.isFrozen = true;
            }
        }
        else
        {
            int damage = (Stack - 50) * 100;
            enemy.TakeSEDamage(Type,damage);
            enemy.isFrozen = false;
            Stack = 0; 
            Debug.Log($"빙결/{Stack}스택 : {damage}");
        }
    
    }

}

// 상태이상 : 독
// 내 턴 종료 시
// 스택 * 0.5% 적 체력 비례 대미지 
// 독 대미지 후 30퍼센트 감소
public class SE_Poison : StatusEffectBase
{
    public override StatusEffectType Type => StatusEffectType.Poison;

    public override void OnTurnEnd(EffectExecutor executor)
    {
        float damageFloat = executor.enemy.enemyMaxHealth * (Stack * 0.005f);
        int damage = Mathf.Max(1, Mathf.FloorToInt(damageFloat));

        executor.enemy.TakeSEDamage(Type,damage);
        Stack = (int)(Stack * 0.7f);
        Debug.Log($"독/{Stack}스택 : {damage}");
    }
}

// 상태이상 : 감전
// 플레이어의 공격이 히트 시 발동
// 턴 종료 시 스택 30퍼 감소
public class SE_Shock : StatusEffectBase
{
    public override StatusEffectType Type => StatusEffectType.Shock;

    public override void OnDealDamage(EffectExecutor executor)
    {
        int damage = Stack;
        executor.enemy.TakeSEDamage(Type, damage);
        Debug.Log($"감전/{Stack}스택 : {damage}");
    }

    public override void OnTurnEnd(EffectExecutor executor)
    {
        Stack = (int)(Stack * 0.7f);
    }
}