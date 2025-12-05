using UnityEngine;

public class UI_MiasmaOverflow : MonoBehaviour
{
    [SerializeField] Animator OverflowAnimator;

    public void PlayOverflow()
    {
        OverflowAnimator.SetTrigger("Damned");
    }
}
