using UnityEngine;

public class ResetStageProgress : MonoBehaviour
{
    [SerializeField] RunInfo runInfo;
    public void InitProgress()
    {
        runInfo.InitProgress();
    }
}
