using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    public void PlaySound()
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource 또는 Clip이 비어있습니다!");
        }
    }
}
