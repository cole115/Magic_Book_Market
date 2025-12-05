using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource source; // BGM AudioSource
    public AudioClip[] bgmList; // 여러 BGM 넣을 리스트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(int index)
    {
        if (source.clip == bgmList[index] && source.isPlaying)
            return;   // 같은 클립 + 재생 중이면 유지

        source.clip = bgmList[index];
        source.Play();
    }


}
