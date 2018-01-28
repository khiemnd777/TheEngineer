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
    public Sprite rightArrowSprite;
    public Sprite bottomArrowSprite;
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

        Create("Prefab object A", "Prefab object A", true, null, prefabPart);
        Create("Prefab object B", "Prefab object B", true, null, prefabPart);

        Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        var pixel = FindObjectOfType<Pixel>();
        Create("Pixel 1", "Pixel 1", true, pixel.gameObject, pixelPart);
        var groups = Create("Group 1", "Group 1", true, null, pixelPart);
        Create("Pixel 2", "Pixel 2", true, null, groups);
        Create("Pixel 3", "Pixel 3", true, null, groups);

        prefabPart.Collapse();
        scriptPart.Collapse();
        pixelPart.Collapse();

        Order();

        // It seems to be non-necessary
        // StartCoroutine(DetectHierarchyItemEntered());
        StartCoroutine(DetectHierarchyItemShowArrow());
    }

    public HierarchyItem Create(string name, string label, bool draggable = true, GameObject reference = null, HierarchyItem parent = null)
    {
        var itemFromResource = Resources.Load<HierarchyItem>(Constants.HIERARCHY_ITEM_PREFAB);
        var instanceItem = Instantiate<HierarchyItem>(itemFromResource, Vector3.zero, Quaternion.identity);
        var textItem = instanceItem.text;
        textItem.text = label;
        instanceItem.draggable = draggable;
        instanceItem.name = name;
        instanceItem.reference = reference;
        if (parent.IsNotNull())
        {
            instanceItem.SetParent(parent);
        }
        // item contains into container
        instanceItem.transform.SetParent(container.transform);
        instanceItem.transform.localScale = Vector3.one;
        instanceItem.onDragEndEvent += (sender, dragEndPosition) => OnHierarchyItemDragEnd(sender, dragEndPosition);
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
        return items.Any(x => x.parent.IsNotNull() && x.parent.GetID() == itemId);
    }

    public void Order()
    {
        var index = -1;
        System.Action<HierarchyItem> ordering = (root) =>
        {
            root.transform.SetSiblingIndex(++index);
            var children = GetChildren(root.GetID());
            foreach (var item in children)
            {
                item.transform.SetSiblingIndex(++index);
            }
            children = null;
        };
        ordering(prefabPart);
        ordering(scriptPart);
        ordering(pixelPart);

        ordering = null;
    }

    void OnHierarchyItemDragEnd(object sender, Vector3 dragEndPosition)
    {
        var itemEntered = GetHierarchyItemEntered();
        if (itemEntered.IsNull())
            return;
        var draggedItem = sender as HierarchyItem;
        DragItemIntoItem(draggedItem, itemEntered);
        draggedItem = null;
        itemEntered = null;
    }

    void DragItemIntoItem(HierarchyItem source, HierarchyItem destination)
    {
        var sourceRef = source.reference;
        if(sourceRef.IsNull())
            return;

        var destId = destination.GetID();
        if (destId == prefabPart.GetID())
        {
            // destination is Prefab part
            // source must be a pixel type
            var sourceRefGo = sourceRef.gameObject;
            var prefabricated = PrefabManager.instance.GetPrefabricated(sourceRefGo);
            if(prefabricated.isPrefab){
                prefabricated = null;
                sourceRefGo = null;
                return;
            }
            var prefabRef = PrefabManager.instance.Create(sourceRefGo);
            var prefabName = prefabRef.name;
            Create(prefabName, prefabName, true, prefabRef, destination);
            Order();
            prefabRef = null;
        }
        else if (destId == scriptPart.GetID())
        {
            // destination is Script part
            // have no idea
        }
        else if (destId == pixelPart.GetID())
        {
            // destination Pixel part
            // duplicate of any prefab type
            var sourceRefGo = sourceRef.gameObject;
            var prefabricated = PrefabManager.instance.GetPrefabricated(sourceRefGo);
            if(!prefabricated.isPrefab){
                prefabricated = null;
                sourceRefGo = null;
                return;
            }
            var unprefabGoRef = PrefabManager.instance.Unprefab(sourceRefGo);
            var unprefabName = unprefabGoRef.name;
            Create(unprefabName, unprefabName, true, unprefabGoRef, destination);
            Order();
            unprefabGoRef = null;
            // or sorting maybe
        }
        sourceRef = null;
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
                // do any stuff here...
                itemEntered = null;
            }
            yield return null;
        }
    }

    IEnumerator DetectHierarchyItemShowArrow()
    {
        while (gameObject.IsNotNull())
        {
            foreach (var item in items)
            {
                item.arrowImage.enabled = AnyChild(item.GetID());
            }
            yield return null;
        }
    }

    void CreatePrefabPart()
    {
        prefabPart = Create(Constants.HIERARCHY_PREFAB_PART, Constants.HIERARCHY_PREFAB_PART_LABEL, false);
    }

    void CreateScriptPart()
    {
        scriptPart = Create(Constants.HIERARCHY_SCRIPT_PART, Constants.HIERARCHY_SCRIPT_PART_LABEL, false);
    }

    void CreatePixelPart()
    {
        pixelPart = Create(Constants.HIERARCHY_PIXEL_PART, Constants.HIERARCHY_PIXEL_PART_LABEL, false);
    }
}
