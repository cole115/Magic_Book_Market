using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    public AudioSource bgmSource;  // 배경음악 AudioSource
    public Text musicText;         // 버튼에 표시될 텍스트

    private bool isMusicOn = true; // 처음에는 음악 켜진 상태라고 가정

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            bgmSource.Play();
            musicText.text = "음악 : 켬";
        }
        else
        {
            bgmSource.Stop();
            musicText.text = "음악 : 끔";
        }
    }
}
