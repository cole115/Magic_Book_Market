using TMPro;
using UnityEngine;

public class UI_DescriptionCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI uprText;
    [SerializeField] TextMeshProUGUI revText;



    [SerializeField] GameObject uprCover;
    [SerializeField] GameObject revCover;

    public void ShowDescription(string name, string upr, string rev,CardFace face)
    {
        cardName.text = name;
        uprText.text = upr;
        revText.text = rev;

        if (face == CardFace.Upright)
        { 
            uprCover.SetActive(true);
            revCover.SetActive(false);
        }
        else
        {
            uprCover.SetActive(false);
            revCover.SetActive(true);
        }
    }

}