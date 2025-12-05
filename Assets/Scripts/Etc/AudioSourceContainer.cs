using UnityEngine;

public class AudioSourceContainer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();    
    }
    public void PlayAudio(int index)
    {
        var clip = audioClips[index];
        if (clip != null)
        {
            audioSource.clip = clip;

            audioSource.Play();
        }   
        
    }
}
