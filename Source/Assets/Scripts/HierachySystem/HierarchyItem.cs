using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyItem : MonoBehaviour, IPointerEnterHandler
{
    // private List<HierarchyItem> _items;

    // public List<HierarchyItem> items
    // {
    //     get
    //     {
    //         return _items ?? (_items = new List<HierarchyItem>());
    //     }
    // }

    public HierarchyItem parent;
    public GameObject reference;

    public void SetParent(HierarchyItem parent)
    {
        this.parent = parent;
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log(this);
	}
}
