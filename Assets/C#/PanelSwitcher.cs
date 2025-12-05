using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private GameObject nextPanel;

    public void SwitchPanel()
    {
        // 현재 패널 끄기
        currentPanel.SetActive(false);

        // 다음 패널 켜기
        nextPanel.SetActive(true);
    }
}
