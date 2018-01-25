using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // private List<HierarchyItem> _items;

    // public List<HierarchyItem> items
    // {
    //     get
    //     {
    //         return _items ?? (_items = new List<HierarchyItem>());
    //     }
    // }

    [System.NonSerialized]
    public HierarchyItem parent;
    [System.NonSerialized]
    public GameObject reference;
    [System.NonSerialized]
    public bool pointerEntered;

    public Text text
    {
        get
        {
            return GetComponentInChildren<Text>();
        }
    }

    public int siblingIndex
    {
        get
        {
            return transform.GetSiblingIndex();
        }
    }

    public void SetParent(HierarchyItem parent)
    {
        this.parent = parent;
        var parentSiblingIndex = parent.siblingIndex;
        var textPosition = parent.text.transform.localPosition;
        var x = textPosition.x + Constants.HIERARCHY_ITEM_SPACE_LEVEL;
        var pos = text.transform.localPosition;
        text.transform.localPosition = new Vector3(x, pos.y, pos.z);
        transform.SetSiblingIndex(parentSiblingIndex + 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerEntered = false;
    }
}
