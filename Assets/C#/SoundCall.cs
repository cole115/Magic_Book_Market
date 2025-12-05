using UnityEngine;

public class SoundCall : MonoBehaviour
{
    public string key;  // 재생할 SFX 키

    public void Play()
    {
        SFXPlayer.Play(key);
    }
}
