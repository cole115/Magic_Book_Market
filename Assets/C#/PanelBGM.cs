using UnityEngine;

public class MainPanelBGM : MonoBehaviour
{
    public int bgmIndex; // Bgm List에서 사용할 인덱스



    private void OnEnable()
    {
        if (BGMManager.Instance == null) return;
        BGMManager.Instance.PlayBGM(bgmIndex);
    }


}

