using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_RerollBtn : MonoBehaviour
{
    Button rerollButton;

    public event Action OnRerollPressed;


    private void Awake()
    {
        rerollButton = GetComponent<Button>();

        rerollButton.onClick.AddListener(() =>
        {
            OnRerollPressed?.Invoke();

        });

        rerollButton.onClick.AddListener(() =>
        {
            PlayAudio();

        });

    }
    private void PlayAudio()
    {
        var container = GetComponent<AudioSourceContainer>();

        container.PlayAudio(0);
    }
}
