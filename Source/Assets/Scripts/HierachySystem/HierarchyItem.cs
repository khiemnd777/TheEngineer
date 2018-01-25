using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [System.NonSerialized]
    public HierarchyItem parent;
    [System.NonSerialized]
    public GameObject reference;
    [System.NonSerialized]
    public bool pointerEntered;
    [System.NonSerialized]
    public bool expanded;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HierarchyManager.instance.AnyChild(this.GetID()))
        {
            if (!expanded)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }
    }

    public void Collapse()
    {
        expanded = false;
        var children = HierarchyManager.instance.GetChildren(this.GetID());
        children.ForEach(x => 
        {
            x.Collapse();
            x.gameObject.SetActive(false);
        });
        children = null;
    }

    public void Expand()
    {
        expanded = true;
        var children = HierarchyManager.instance.GetChildren(this.GetID());
        children.ForEach(x => 
        {
            x.gameObject.SetActive(true);
        });
        children = null;
    }
}
