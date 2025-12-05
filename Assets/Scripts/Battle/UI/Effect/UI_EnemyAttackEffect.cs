using UnityEngine;

public class UI_EnemyAttackEffect : MonoBehaviour
{
    [SerializeField] Animator AttackEffectAnimator;


    public void PlayEnemyAttack(int attackDir)
    {
        AttackEffectAnimator.SetTrigger(attackDir.ToString());
    }

}
