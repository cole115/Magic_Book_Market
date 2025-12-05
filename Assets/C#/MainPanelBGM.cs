using UnityEngine;

public class PanelBGM : MonoBehaviour
{
    public int bgmIndex;

    private void OnEnable()
    {
        if (BGMManager.Instance == null) return;
        BGMManager.Instance.PlayBGM(bgmIndex);
    }
}
