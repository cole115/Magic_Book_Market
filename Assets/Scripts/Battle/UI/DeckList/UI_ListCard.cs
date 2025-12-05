using UnityEngine;
using UnityEngine.UI;
public class UI_ListCard : MonoBehaviour
{
    [SerializeField] Sprite placeHolder;
    [SerializeField] Image cardImage;

    GameCard card;
    public async void BindCard(GameCard c)
    {
        card = c;

        string cardID = c.Card.CardID;
        string spriteCode = cardID[^3..];
        if (!int.TryParse(spriteCode, out int num))
        {
            Debug.LogError($"스프라이트 로드 에러 : CardID가 잘못됨 {cardID}");
            return;
        }
        Sprite sprite = await CardDatabase.GetSprite(spriteCode);

        cardImage.sprite = sprite == null? placeHolder : sprite;

    }

    public void Clear()
    {
        card = null;
        cardImage.sprite = placeHolder;
        
    }

}
