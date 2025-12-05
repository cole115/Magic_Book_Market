using System;
using UnityEngine;

public class UI_PlayerAttackEffect : MonoBehaviour
{

    [SerializeField] Animator AttackEffectAnimator;
    public void PlayAttackEffect(CardElement element)
    {
        AttackEffectAnimator.SetTrigger(element.ToString());

        
    }

}
