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

        List<float> validHitChances = HitChanceList.Where(h => h != 0).ToList();    // 공격 확률 리스트에서 0이 아닌걸 리스트로
        List<int> directions = new List<int> { 0, 1, 2, 3 };                        // 공격 방향 리스트

        for (int i = 0; i < validHitChances.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, directions.Count);        // 랜덤 공격 방향 지정
            int chosenDir = directions[randomIndex];

            directions.RemoveAt(randomIndex);                                       // 이미 지정된 공격 방향 리스트에서 삭제

            instances.Add(new AttackInstance                                        
            {
                Direction = chosenDir,
                HitChance = validHitChances[i],
                Multiplier = DamageMultiplier
            });                                                                     // 공격 방향에 공격정보 추가
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

 