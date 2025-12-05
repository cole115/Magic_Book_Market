using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ResultCardOutliner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image ThisImage;
    [SerializeField] Material CardMaterial;

    [HideInInspector]public int slotNum;
    public event Action<ResultCardOutliner> OnRequestSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnRequestSelected?.Invoke(this);
    }
    public void SetSelected(bool selected)
    {
        ThisImage.material = selected ? CardMaterial : null;
    }

}
