using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class UI_ResultCanvas : MonoBehaviour
{
    [SerializeField] Canvas ResultCanvas;
    [SerializeField]
    GameObject[] RewardCards = new GameObject[3];

    [Header("Gold")]
    [SerializeField] TextMeshProUGUI GoldText;

    [Header("Result")]
    [SerializeField] Image WinOrLose;
    [SerializeField] Sprite Win;
    [SerializeField] Sprite Lose;


    [Header("Button")]
    [SerializeField] Button ConfirmBtn;

    private UI_ListCard[] listCards = new UI_ListCard[3];
    private ResultCardOutliner[] outliners = new ResultCardOutliner[3];


    private bool isWin;

    private int rewardSelected = -1;
    private int winGold;

    [SerializeField] GameObject finalResult;
    [SerializeField] Button finalConfirm;
    [SerializeField] RunInfo runInfo;
    private bool allClear = false;


    // BattleUIPresenter에서 호출
    public void ShowResult(bool isWin, int gold)
    {
        gameObject.SetActive(true);



        this.isWin = isWin;
        winGold = gold;

        for (int i = 0; i < RewardCards.Length; i++)
        {
            listCards[i] = RewardCards[i].GetComponent<UI_ListCard>();
            outliners[i] = RewardCards[i].GetComponent<ResultCardOutliner>();

            outliners[i].slotNum = i;
        }

        for (int i = 0; i < listCards.Length; i++)
        {
            var gameCard = CardDatabase.GetRandomCard();

            listCards[i].BindCard(gameCard);
        }

        // 전부 클리어 시 보여줄 임시 코드
        if (runInfo.Stage == 7)
        {
            allClear = true;
            finalResult.SetActive(true);
            finalConfirm.onClick.AddListener(MoveNext);
            return;
        }

        ConfirmBtn.onClick.AddListener(MoveNext);


        foreach (var c in outliners)
            c.OnRequestSelected += OnCardRequestSelect;

        EnableConfirmBtn(false);
        ShowWinOrLose();    
        GoldText.text = winGold.ToString() + " G";
    }

    private void EnableConfirmBtn(bool enable)
    {
        ConfirmBtn.interactable = enable;
        Image btnImage =  ConfirmBtn.transform.GetComponentInChildren<Image>();
        Color targetColor = enable ? 
            new Color(1.0f, 1.0f, 1.0f) : new Color(0.7f, 0.7f, 0.7f);

        btnImage.color = targetColor;

        ConfirmBtn.transform.GetComponent<ButtonScaleChanger>().enabled = enable;
    }

    private void ShowWinOrLose()
    {
        if (isWin)
            WinOrLose.sprite = Win;
        else
            WinOrLose.sprite = Lose;
    }



    private void OnCardRequestSelect(ResultCardOutliner outliner)
    {
        int requestNum = outliner.slotNum;

        if (rewardSelected == requestNum)
            return;

        rewardSelected = requestNum;
        for (int i = 0; i < outliners.Length; i++)
        {
            bool isSelected = (i == rewardSelected);
            outliners[i].SetSelected(isSelected);
        }

        if(!ConfirmBtn.interactable)
        {
            EnableConfirmBtn(true);
        }
    }

    public void MoveNext()
    {
        var loader = GetComponent<SceneLoader>();
        if (!allClear)
        {
            Debug.Log("다음 스테이지로");
            loader.LoadSceneByName("sime");

        }
        else
            loader.LoadSceneByName("SampleScene");
    }



}
