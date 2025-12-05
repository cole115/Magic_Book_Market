using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class UI_DeckListCanvas : MonoBehaviour
{
    [SerializeField] List<UI_ListCard> slotList;       // 카드 슬롯. 3*3 보여주고, 위아래 3줄 여유를 주기 위해 총 18개 슬롯
    [SerializeField] Canvas deckListCanvas; 
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform contentGrid;

    List<GameCard> deckList = new();

    // 슬롯 높이 350
    float cellHeight = 350;


    int rowVisible = 3; // 화면에 보일 행 3줄
    int bufferRows = 2; // 화면 밖 위아래 2줄씩


    int columns = 3;    // 열 3줄
    int totalRowsInPool = 7; // 풀에 있는 줄은 총 7줄(rowVisible + bufferRows*2)

    int totalRows;  // 모든 리스트를 행으로 치환했을 때의 수
    

    int currentTopRow = 0;


    public void OpenDeckList(List<GameCard> deck)
    {
        var sorted = ReturnSortedDeck.SortDeck(deck);
        deckList = sorted;

        var totalRows = Mathf.CeilToInt(deckList.Count / (float)columns);

        ResizeContent();
        InitSlots();

        deckListCanvas.enabled = true;
    }

    // 그리드 높이 조정
    private void ResizeContent()
    {
        float width = contentGrid.sizeDelta.x;
        float height = totalRows * cellHeight;

        contentGrid.sizeDelta = new Vector2(width, height);
    }

    // 슬롯 초기화
    private void InitSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            int dataIndex = i;
            if(dataIndex < deckList.Count)  // 해당 카드가 슬롯 개수 범위 안에 있을 경우
            {
                slotList[i].BindCard(deckList[dataIndex]);
            }
            else
            {
                slotList[i].Clear();       // 슬롯 범위 밖일 경우 데이터 제거
            }
        }
    
    
    
    }



    // 덱리스트 끄기. 캔버스 안의 ExitTouchArea에서 처리
    public void CloseDeckList()
    {
        deckListCanvas.enabled = false;
    }





    // 인스펙터의 ScrollRect에 넣을 함수
    public void OnListScroll(Vector2 pos)
    {
        float contentY = contentGrid.anchoredPosition.y;

        int newTopRow = Mathf.FloorToInt(contentY / cellHeight);

        if (newTopRow != currentTopRow)
        {
            if (newTopRow > currentTopRow)
                ScrollDown(newTopRow - currentTopRow);
            else
                //ScrollUp(currentTopRow - newTopRow);
            
            currentTopRow = newTopRow;
                   
        }

    }


    void ScrollDown(int rowCount)
    {
        for(int r=0; r<rowCount; r++)
        {
            MoveTopRowToBottom();
            
        }

    }


    // 윗줄을 아랫줄로 이동
    private void MoveTopRowToBottom()
    {
        for (int i = 0; i < columns; i++)   // 0 -> 1 -> 2
        {
            var slot = slotList[0];
            slotList.RemoveAt(0);
            slotList.Add(slot);

            slot.transform.SetAsLastSibling();

        }
    }

    private void BindBottomRow()
    {
        int startDataIndex = (currentTopRow + bufferRows + rowVisible) * columns;

        for (int col = 0; col < columns; ++col)
        {
            int slotIndex = slotList.Count - columns + col;
            int dataIndex = startDataIndex + col;
        
        
        }
    }



    // 아랫줄을 윗줄로 이동
    private void MoveBottomRowToTop()
    {
        for (int i = columns - 1; i >= 0; i--)  // 2 -> 1 -> 0
        {
            var slot = slotList[slotList.Count - 1];
            slotList.RemoveAt(slotList.Count - 1);
            slotList.Insert(0, slot);
            
            slot.transform.SetAsFirstSibling();
        }
    }





    private void UpdateSlots(int newTopRow)
    {
        // 로드 범위의 제일 첫번째 인덱스
        int startIndex = (currentTopRow - bufferRows) * columns;
        if (startIndex < 0) startIndex = 0;

        for (int row = 0; row < bufferRows; row++)
        {
            for(int col = 0; col < columns; col++)
            {
                slotList[col].transform.SetAsLastSibling();
            }
        }




        int totalRows = Mathf.CeilToInt(deckList.Count / columns);
    }




}
