using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyManager : MonoBehaviour
{
    #region Singleton
    static HierarchyManager _instance;

    public static HierarchyManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<HierarchyManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active HierarchyManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public HierarchyItem itemPrefab;
    public RectTransform container;

    List<HierarchyItem> _items;

    public List<HierarchyItem> items
    {
        get
        {
            return _items ?? (_items = new List<HierarchyItem>());
        }
    }

    void Start()
    {
        var scripts = Create("Scripts");
        Create("instance-script.py", null, scripts);
        Create("instance-script.py", null, scripts);
        Create("instance-script.py", null, scripts);
        var pixels = Create("Pixels");
        Create("Pixel 1", null, pixels);
        var groups = Create("Group 1", null, pixels);
        Create("Pixel 2", null, groups);
        Create("Pixel 3", null, groups);
    }

    void Update()
    {
        var itemEntered = GetHierarchyItemEntered();
        if (itemEntered.IsNotNull())
            Debug.Log(itemEntered.text.text);
    }

    public HierarchyItem Create(string name, GameObject reference = null, HierarchyItem parent = null)
    {
        var itemFromResource = Resources.Load<HierarchyItem>(Constants.HIERARCHY_ITEM_PREFAB);
        var instanceItem = Instantiate<HierarchyItem>(itemFromResource, Vector3.zero, Quaternion.identity);
        var textItem = instanceItem.text;
        textItem.text = name;
        instanceItem.name = name;
        instanceItem.reference = reference;
        if (parent.IsNotNull())
        {
            instanceItem.SetParent(parent);
        }
        // item contains into container
        instanceItem.transform.SetParent(container.transform);
        instanceItem.transform.localScale = Vector3.one;
        items.Add(instanceItem);
        // release memory
        textItem = null;
        itemFromResource = null;
        return instanceItem;
    }

    public void SetParent(HierarchyItem item, HierarchyItem parent)
    {
        item.SetParent(parent);
    }

    HierarchyItem GetHierarchyItemEntered()
    {
        var itemEntered = _items.FirstOrDefault(x => x.pointerEntered);
        return itemEntered;
    }
}
