using UnityEngine;

public enum ModifyType
{
    AccesoryBuff = 0,
    ReversedCardBuff = 1,
    ActiveCardBuff = 2
}





public interface Modifier
{
    ModifyType Type { get; }
    int ModifyDamage(int currentDamage, CardElement element);
    string ModifyDescription(string desc, CardElement element);
}


public class AttackModifier : Modifier 
{
    public ModifyType Type => ModifyType.ReversedCardBuff;

    private float _multiplier;

    public AttackModifier(float multiplier)
    {
        _multiplier = multiplier;
    }

    public int ModifyDamage(int currentDamage, CardElement element)
    {
        return (int)(currentDamage * _multiplier);

    }
    public string ModifyDescription(string desc, CardElement element)
    {
        return $"{element} °­È­ : " + desc;
    }

}