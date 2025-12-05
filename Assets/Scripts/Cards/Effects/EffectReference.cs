using UnityEngine;

//public enum 

public class EffectReference
{
    public int CurrentTurn()
    {
        return MasterBattleManager.Instance.Processor.turnCount;
    }

    //public int RemainingMana()
    //{

    //}

}
