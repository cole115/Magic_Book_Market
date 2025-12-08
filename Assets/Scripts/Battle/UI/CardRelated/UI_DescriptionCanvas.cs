using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DescriptionCanvas : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] Image cardImage;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI cardElement;
    [SerializeField] TextMeshProUGUI cardCost;
    [SerializeField] TextMeshProUGUI uprText;
    [SerializeField] TextMeshProUGUI revText;

    [Header("Cover")]
    [SerializeField] GameObject uprCover;
    [SerializeField] GameObject revCover;

    public void ShowDescription(GameCard card)
    {
        cardImage.sprite = card.Card.CardSprite;

        cardName.text = card.Card.Name;
        ColorUtility.TryParseHtmlString(ColorCode(card.Card.Rank), out var color);
        cardName.color = color;

        cardElement.text = "Type : " + Element(card.Card.Element);
        cardCost.text = "Cost : " + card.Card.Effects.Cost.ToString();
        uprText.text = card.Card.Effects.UprDescription;
        revText.text = Trigger(card.Card.Effects.Trigger) + " : " + card.Card.Effects.RevDescription;

        if (card.CardFace == CardFace.Upright)
        {
            uprCover.SetActive(false);
            revCover.SetActive(true);
        }
        else
        {
            uprCover.SetActive(true);
            revCover.SetActive(false);            
        }


        gameObject.SetActive(true);

    }


    public void HideDescription()
    {
        gameObject.SetActive(false);
    }

    private string ColorCode(CardRank rank)
    {
        string code = rank switch
        {
            CardRank.Normal => "#FFFFFF",       //    FFFFFF 일반 흰색
            CardRank.Rare => "#92FF72",         //    92FF72 희귀 녹색
            CardRank.Hero => "#7184FF",         //    7184FF 영웅 파란색
            CardRank.Legend => "FFEE3B",        //    FFEE3B 전설 노란색
            _ => null
        };

        return code;
    }

    private string Element(CardElement e)
    {
        string element = e switch
        {
            CardElement.None => "무",
            CardElement.Fire => "불",
            CardElement.Ice => "얼음",
            CardElement.Grass => "풀",
            CardElement.Lightning => "번개",
            _ => null

        };
        return element;
    }

    private string Trigger(RevEffectTrigger t)
    {
        string trigger = t switch
        {
            RevEffectTrigger.Continuous => "다른 카드 사용 시",
            RevEffectTrigger.TurnEnd => "턴 종료 시",
            _ => null
        };
        return trigger;
    }
}



