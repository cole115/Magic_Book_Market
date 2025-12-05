using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndTurnBtn : MonoBehaviour
{
    Button endTurnButton;

    public event Action OnEndTurnPressed;

    private void Awake()
    {
        endTurnButton = GetComponent<Button>();



        endTurnButton.onClick.AddListener(() =>
        { OnEndTurnPressed?.Invoke(); 
        });

        endTurnButton.onClick.AddListener(() =>
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
