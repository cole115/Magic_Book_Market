using UnityEngine;

public static class SFXPlayer
{
    public static void Play(string key)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.Play(key);
        }
        else
        {
            Debug.LogWarning("SFXManager 없음: 초기화 전입니다.");
        }
    }
}
