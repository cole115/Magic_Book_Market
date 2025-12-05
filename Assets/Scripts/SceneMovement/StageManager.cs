using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    [SerializeField] RunInfo runInfo;

    [SerializeField] GameObject Map;
    [SerializeField] GameObject CaseTab;
    [SerializeField] GameObject RewardTab;
    [SerializeField] GameObject ShopTab;


    [SerializeField] TextMeshProUGUI stageText;

    private void Awake()
    {
        stageText.text = $"현재 오른 층 : {runInfo.Stage}";
            
    }


    public void MoveToNextStage()
    {
        if (runInfo == null)
        {
            Debug.LogError("RunInfo가 없습니다");
            return;
        }
        int nextStage = runInfo.AddStageNum();

        switch(nextStage)
        {
            case 1: OpenBattleStage(nextStage); break;
            case 2: OpenCaseStage();break;
            case 3: OpenBattleStage(nextStage); break;
            case 4: OpenStoreStage();break;
            case 5: OpenRewardStage();break;
            case 6: OpenBattleStage(nextStage); break;
        }


    }

    public void OpenMap()
    {
        Map.SetActive(true);
        ShopTab.SetActive(false);
        RewardTab.SetActive(false);
        CaseTab.SetActive(false);

        stageText.enabled = true;
        stageText.text = $"현재 오른 층 : {runInfo.Stage}";
    }


    public void OpenCaseStage()
    {
        Map.SetActive(false);
        ShopTab.SetActive(false);
        RewardTab.SetActive(false);
        CaseTab.SetActive(true);

        stageText.enabled = false;
        
    }

    public void OpenRewardStage()
    {
        Map.SetActive(false);
        ShopTab.SetActive(false);
        RewardTab.SetActive(true);
        CaseTab.SetActive(false);

        stageText.enabled = false;
    }

    public void OpenStoreStage()
    {
        Map.SetActive(false);
        ShopTab.SetActive(true);
        RewardTab.SetActive(false);
        CaseTab.SetActive(false);

        stageText.enabled = false;
    }


    public void OpenBattleStage(int stageNum)
    {
        string enemy = stageNum switch
        {
            1 => "M1001",
            3 => "M1010",
            6 => "M1019",
            _ => throw new System.Exception("잘못된 스테이지 번호")
        };
        
        runInfo.SetEnemy(enemy);

        var sceneLoader = GetComponent<SceneLoader>();
        sceneLoader.LoadSceneByName("BattleScene");
       
    }
}
