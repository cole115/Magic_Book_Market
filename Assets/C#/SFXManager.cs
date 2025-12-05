using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource source;              // 효과음 재생용 오디오소스
    public List<SFXData> sfxList;           // 효과음 데이터 리스트

    private Dictionary<string, AudioClip> sfxDict;

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

        // 리스트 → 딕셔너리로 변환
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var item in sfxList)
        {
            sfxDict[item.key] = item.clip;
        }
    }

    public void Play(string key)
    {
        if (sfxDict.ContainsKey(key))
        {
            source.PlayOneShot(sfxDict[key]);
        }
        else
        {
            Debug.LogWarning($"SFX '{key}' 없음!");
        }
    }
}

[System.Serializable]
public class SFXData
{
    public string key;      // "button", "hit", "buy" 같은 이름
    public AudioClip clip;  // 실제 오디오 파일
}
