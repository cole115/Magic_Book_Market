using System.Linq;
using UnityEngine;

// 회피 결과
public struct EvadeResult
{
    public bool IsEvaded;
    public bool IsCardDestroyed;

}

// 회피 계산기
public static class EvadeCalculator
{
    // HitChance = 적의 명중률.
    // secondChance = 플레이어의 회피율
    public static EvadeResult TryEvade(AttackInstance attack, GameCard[] handSlots)
    {
        var result = new EvadeResult();

        float hitChance = attack.HitChance * 0.01f;


        // 1차 회피 성공
        if (UnityEngine.Random.value > hitChance)
        {
            result.IsEvaded = true;
            result.IsCardDestroyed = false;
            return result;
        }

        // 1차 회피 실패. 카드 파괴
        if (handSlots[attack.Direction] != null)
        {
            result.IsEvaded = false;
            result.IsCardDestroyed = true;
            return result;
        }

        // 2차 회피 시도
        int emptyCount = handSlots.Count(x => x == null);

        float secondChance = emptyCount switch
        {
            1 => 0.8f,
            2 => 0.5f,
            3 => 0.3f,
            5 => 0.1f,
            _ => 0f
        };

        bool isEvaded = UnityEngine.Random.value <= secondChance;
        result.IsEvaded = isEvaded;
        result.IsCardDestroyed = false;

        return result;

    }

}
