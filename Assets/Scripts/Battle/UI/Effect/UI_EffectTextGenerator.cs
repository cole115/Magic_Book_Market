using UnityEngine;

public class UI_EffectTextGenerator : MonoBehaviour
{
    [SerializeField] GameObject TextPrefab;
    public void ShowEffectText(EffectType type, int amount, CardElement element)
    {
        string effectText = TextToKorean(type, amount, element);
        GenerateText(effectText);
    }

    public void ShowSEText(StatusEffectType type, int amount)
    {
        string seText = $"{SEToKorean(type)} {amount}대미지";
        GenerateText(seText);
    }

    private void GenerateText(string message)
    {
        Debug.Log("-------------"+message+"-------------------");
        var textPrefab = Instantiate(TextPrefab,this.transform);
        var uiText = textPrefab.GetComponent<UI_EffectText>();
        StartCoroutine(uiText.DisplayText(message));
    }

    private string TextToKorean(EffectType type, int amount, CardElement element)
    {
        string text = type switch
        {
            EffectType.DealDamage => $"{amount} 대미지",
            EffectType.IncreaseAttack => $"+{amount} 공격력",
            EffectType.InflictSE => $"{SEToKorean(ElementToSE(element))} {amount}스택",
            EffectType.DrawCard => $"{amount} 드로우",
            EffectType.AddReroll => $"+{amount} 리롤",
            EffectType.HealMiasma => $"-{amount} 오염도",
            EffectType.RecovoerMana => $"+{amount} 마나",
            _ => throw new System.Exception("잘못된 효과 값")
        };

        return text;
    }

    private StatusEffectType ElementToSE(CardElement element)
    {
        StatusEffectType type = element switch
        {
            CardElement.Fire => StatusEffectType.Burn,
            CardElement.Ice => StatusEffectType.Freeze,
            CardElement.Grass => StatusEffectType.Poison,
            CardElement.Lightning => StatusEffectType.Shock,
            _ => throw new System.Exception("잘못된 속성 값")
        };

        return type;
    }


    private string SEToKorean(StatusEffectType type)
    {
        string text = type switch
        {
            StatusEffectType.Burn => "화상",
            StatusEffectType.Freeze => "빙결",
            StatusEffectType.Poison => "독",
            StatusEffectType.Shock => "감전",
            _ => throw new System.Exception("잘못된 상태이상 값")
        };

        return text;
    }

}

