using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "RunInfo", menuName = "Scriptable Objects/RunInfo")]
public class RunInfo : ScriptableObject
{
    public List<GameCard> DeckList { get; private set; }

    public int Miasma { get; private set; }
    public string EnemyName { get; private set; }
    public int Gold { get; private set; }   

    public int Stage { get; private set; }
    

    public void InitProgress()
    {
        Stage = 0;
    }
    public void InitDeck()
    {
        if (DeckList == null)
            DeckList = new List<GameCard>();
    }


    // 덱에 카드를 추가
    public void AddCardToDeck(GameCard card)
    {
        InitDeck();

        if (DeckList.Any(c => c.CardGuid == card.CardGuid))
        {
            Debug.LogError("카드 GUID 중복");
            return;
        }
        DeckList.Add(card);

    }

    // 덱의 카드를 지움
    // 전투 종료 시 삭제된 카드 일괄 지움
    // 그냥 지울 바에 덱 통째로 다시 복사해 넣는게 나을지도?
    public bool RemoveCardFromDeck(GameCard card)
    {
        Guid id = card.CardGuid;

        for (int i = DeckList.Count - 1; i >= 0; i--)
        {
            if (DeckList[i].CardGuid == id)
            {
                DeckList.RemoveAt(i);
                return true;
            }

        }
        return false;
    }

    public void ClearDeck()
    {
        DeckList.Clear();
    }


    public int AddStageNum()
    {
        Stage++;
        return Stage;
    }

    public void SetEnemy(string enemy)
    {
        EnemyName = enemy;
    }

}
