using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceLocations;

// Addressable로 카드 캐싱. 
// 만약 도감이 추가될 일이 생기면 label로 가져올 수 있는 
// IList<IResourceLocation> 찾아보기
public static class CardDatabase
{
    // 카드의 데이터 딕셔너리.
    public static Dictionary<string, CardData> Cards { get; private set; } = new(); 

    // 어드레서블 로드의 Task를 넣은 딕셔너리.
    private static Dictionary<string, AsyncOperationHandle<Sprite>> cache = new();


    // CardSprites 레이블 일괄 로드
    public static async Task LoadAllCards()
    {
        
        var handle = Addressables.LoadAssetsAsync<CardData>("Cards", null);
        await handle.Task;

        Cards.Clear();
        foreach (var card in handle.Result)
        {
            if (!Cards.ContainsKey(card.CardID))
                Cards.Add(card.CardID, card);
        }

        Debug.Log("카드 데이터 로드 완료");

    }
    
    public static GameCard GetRandomCard()
    {
        int randomNum = UnityEngine.Random.Range(1, Cards.Count + 1);
        CardData data = GetCard(randomNum);
        GameCard gameCard = new GameCard(data);
        return gameCard;
    }




    // 데이터베이스에서 카드 불러오기
    public static CardData GetCard(int idNum)
    {
        string formattedNum = idNum.ToString("D3");
        string cardCode = "Cardcode_" + formattedNum;

        if(Cards.TryGetValue(cardCode, out var card))
            return card;

        Debug.LogWarning($"카드 로드 실패 : {cardCode}");
        return null;
    }



    // 스프라이트 로드 Task
    public static async Task<Sprite> GetSprite(string cardID)
    {
        // 로드 된 적 없는 스프라이트일 경우 로드 & 캐시 추가
        if (!cache.TryGetValue(cardID, out var handle))
        {
            handle = Addressables.LoadAssetAsync<Sprite>(cardID);
            cache[cardID] = handle;
        }
        try
        {
            await handle.Task;
            return handle.Result;
        }
        catch
        {
            Debug.LogError($"스프라이트 로드 실패 : {cardID}");
            cache.Remove(cardID);
            return null;
        }
    }

    public static bool IsSpriteCached(string id)
    {
        return cache.ContainsKey(id);
    }



    // 현재로썬 Release 할 필요 없음
    //public static void Release(string addr)
    //{
    //    if (cache.TryGetValue(addr, out var handle))
    //    {
    //        Addressables.Release(handle);
    //        cache.Remove(addr);
    //    }

    //}


    //public static void ReleaseAll()
    //{
    //    foreach(var kvp in  cache)
    //    {
    //        Addressables.Release(kvp.Value);
    //    }
    //    cache.Clear();
    //    Debug.Log("[Preloader] Released all cache");
    //}
}
