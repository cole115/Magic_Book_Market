using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_AttackPreviewArrows : MonoBehaviour
{
    [SerializeField] 
    GameObject[] Arrows = new GameObject[4];

    Image[] arrowImages;
    TextMeshProUGUI[] hitChances;

    private void Awake()
    {
        arrowImages = new Image[Arrows.Length];
        hitChances = new TextMeshProUGUI[Arrows.Length];
        for (int i = 0; i < Arrows.Length; i++)
        {
            arrowImages[i] = Arrows[i].GetComponent<Image>();
            hitChances[i] = Arrows[i].GetComponentInChildren<TextMeshProUGUI>();

        }
        ResetPreview();
    }

    public void PreviewEnemyAttacks(List<AttackInstance> attackInfo)
    {
        ResetPreview();
        foreach(var attack in attackInfo)
        {
            int direction = attack.Direction;
            float hitChance = attack.HitChance;

            Debug.Log($"공격방향 : {attack.Direction},명중률 : {attack.HitChance}");

            if(hitChance >0)
            {
                arrowImages[direction].enabled = true;
                hitChances[direction].enabled = true;
                hitChances[direction].text = $"{hitChance}%";

            }
        }

    }

    public void ResetPreview()
    {
        for (int i = 0; i < Arrows.Length; i++)
        {
            arrowImages[i].enabled = false;
            hitChances[i].enabled = false;

        }
    }

}
