using UnityEngine;
using UnityEngine.UI;

public class MenuSwitcher : MonoBehaviour
{
    enum Tabs
    {
        Shop,
        Home,
        Card
    
    }

    [SerializeField] Tabs currentTab = Tabs.Home;

    [SerializeField] Button ShopBtn;
    [SerializeField] Button HomeBtn;
    [SerializeField] Button CardBtn;


    private void Awake()
    {
        currentTab = Tabs.Home;
    }

    private void Update()
    {
        
    }


    private void ChangeBtnColor(bool isSwitched)
    {
        
    }

}
