using UnityEngine;

public class ScreenResizer : MonoBehaviour
{
    [SerializeField, Range(0.3f, 1f)]
    private float screenScale = 0.7f;



#if UNITY_STANDALONE
    private void Awake()
    {
        SetWindowSize();
    }
#endif


    // 화면 비율 조정
    // 세로 길이가 디스플레이의 70%
    // 가로는 9:16 맞춰 조정
    private void SetWindowSize()
    { 
        int monitorWidth = Display.main.systemWidth;
        int monitorHeight = Display.main.systemHeight;


        int targetHeight = (int)(monitorHeight * screenScale);
        int targetWidth = (int)(targetHeight * 9f / 16f);

        Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);

         

    
    }


}
