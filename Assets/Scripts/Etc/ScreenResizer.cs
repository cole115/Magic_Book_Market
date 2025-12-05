using UnityEngine;

public class ScreenResizer : MonoBehaviour
{
    
#if UNITY_STANDALONE_WIN
    private void Awake()
    {
        Screen.SetResolution(720,1280,FullScreenMode.Windowed);
    }
#endif
}
