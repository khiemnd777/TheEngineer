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
    [System.NonSerialized]
    public HierarchyItem scriptPart;
    [System.NonSerialized]
    public HierarchyItem prefabPart;
    [System.NonSerialized]
    public HierarchyItem pixelPart;

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
        CreatePrefabPart();
        CreateScriptPart();
        CreatePixelPart();
        
        Create("Prefab object A", null, prefabPart);
        Create("Prefab object B", null, prefabPart);
        
        Create("instance-script.py", null, scriptPart);
        Create("instance-script.py", null, scriptPart);
        Create("instance-script.py", null, scriptPart);
        
        Create("Pixel 1", null, pixelPart);
        var groups = Create("Group 1", null, pixelPart);
        Create("Pixel 2", null, groups);
        Create("Pixel 3", null, groups);

        prefabPart.Collapse();
        scriptPart.Collapse();
        pixelPart.Collapse();

        Order();

        StartCoroutine(DetectHierarchyItemEntered());
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

    public void Collapse(HierarchyItem item)
    {
        item.Collapse();
    }

    public void Expand(HierarchyItem item)
    {
        item.Expand();
    }

    public List<HierarchyItem> GetChildren(int itemId)
    {
        var chilren = items.Where(x => x.parent.IsNotNull() && x.parent.GetID() == itemId).ToList();
        return chilren;
    }

    public bool AnyChild(int itemId)
    {
        return items.Any(x=>x.parent.IsNotNull() && x.parent.GetID() == itemId);
    }

    public void Order()
    {
        var workingItems = _items;
        var total = workingItems.Count;
        var index = -1;
        System.Action<HierarchyItem> ordering =(root) => {
            root.transform.SetSiblingIndex(++index);
            var children = GetChildren(root.GetID());
            foreach(var item in children)
            {
                item.transform.SetSiblingIndex(++index);
            }
        };
        ordering(prefabPart);
        ordering(scriptPart);
        ordering(pixelPart);
    }

    HierarchyItem GetHierarchyItemEntered()
    {
        var itemEntered = _items.FirstOrDefault(x => x.pointerEntered);
        return itemEntered;
    }

    IEnumerator DetectHierarchyItemEntered()
    {
        while (gameObject.IsNotNull())
        {
            var itemEntered = GetHierarchyItemEntered();
            if (itemEntered.IsNotNull())
            {
                //  do any stuff here...
                itemEntered = null;
            }
            yield return null;
        }
    }

    void CreatePrefabPart(){
        prefabPart = Create("Prefabs");
    }

    void CreateScriptPart(){
        scriptPart = Create("Scripts");
    }

    void CreatePixelPart(){
        pixelPart = Create("Pixels");
    }
}
