using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Enemy_Pattern : ScriptableObject
{
    public string PatternID;
    public string MonsterID;
    public float DamageMultiplier;
    public List<float> HitChanceList;
    public float UseChance;

    // 공격 정보 생성(공격 방향, 각 공격 별 히트 확률)
    // 대미지 배율도 추가
    public List<AttackInstance> GetAttackInstances()
    {
        List<AttackInstance> instances = new();

        List<float> validHitChances = HitChanceList.Where(h => h != 0).ToList();
        List<int> directions = new List<int> { 0, 1, 2, 3 };

        for (int i = 0; i < validHitChances.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, directions.Count);
            int chosenDir = directions[randomIndex];

            directions.RemoveAt(randomIndex);

            instances.Add(new AttackInstance
            {
                Direction = chosenDir,
                HitChance = validHitChances[i],
                Multiplier = DamageMultiplier
            });
        }
        

        return instances.OrderBy(inst => inst.Direction).ToList();
    }

}


// 공격 정보
// Direction 값은 0 ~ 3
public struct AttackInstance
{
    public int Direction;
    public float HitChance;
    public float Multiplier;
}

 