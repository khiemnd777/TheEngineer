﻿using System.Collections;
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
    PrefabManager prefabManager;
    ScriptManager scriptManager;

    public List<HierarchyItem> items
    {
        get
        {
            return _items ?? (_items = new List<HierarchyItem>());
        }
    }

    void Awake()
    {
        scriptManager = ScriptManager.instance;
        prefabManager = PrefabManager.instance;
    }

    void Start()
    {
        CreatePrefabPart();
        CreateScriptPart();
        // CreatePixelPart();

        // Create("Prefab object A", "Prefab object A", true, null, prefabPart);
        // Create("Prefab object B", "Prefab object B", true, null, prefabPart);

        // Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        // Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        // Create("instance-script.py", "instance-script.py", true, null, scriptPart);
        // var pixel = FindObjectOfType<Pixel>();
        // Create("Pixel 1", "Pixel 1", true, pixel.gameObject, pixelPart, pixelPart);
        // var groups = Create("Group 1", "Group 1", true, null, pixelPart, pixelPart);
        // Create("Pixel 2", "Pixel 2", true, null, groups, pixelPart);
        // Create("Pixel 3", "Pixel 3", true, null, groups, pixelPart);

        // UpdatePixelPart();

        prefabPart.Collapse();
        scriptPart.Collapse();
        // pixelPart.Collapse();

        Order();

        // It seems to be non-necessary
        // StartCoroutine(DetectHierarchyItemEntered());
        StartCoroutine(DetectHierarchyItemShowArrow());
        StartCoroutine(UpdateScriptName());
    }

    public HierarchyItem Create(string name, string label, bool draggable = true, GameObject reference = null, HierarchyItem parent = null, HierarchyItem originalParent = null)
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
            instanceItem.gameObject.SetActive(parent.expanded);
        }
        if(originalParent.IsNotNull()){
            instanceItem.originalParentId = originalParent.GetID();
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
        OrderSibling(prefabPart, ref index);
        OrderSibling(scriptPart, ref index);
        OrderSibling(pixelPart, ref index);
    }

    public void CreatePrefab(GameObject target)
    {
        var sourceRefGo = target;
        if (sourceRefGo.IsNull())
            return;
        var prefabricated = prefabManager.GetPrefabricated(sourceRefGo);
        if (prefabricated.isPrefab)
        {
            prefabricated = null;
            sourceRefGo = null;
            return;
        }
        var prefabRef = prefabManager.Create(sourceRefGo);
        var prefabName = prefabRef.name;
        Create(prefabName, prefabName, true, prefabRef, prefabPart, prefabPart);
        Order();
        prefabRef = null;
        sourceRefGo = null;
    }

    public void CreateScript(GameObject target)
    {
        var sourceRefGo = target;
        if(sourceRefGo.IsNull())
            return;
        var scriptName = target.name;
        Create(scriptName, scriptName, true, target, scriptPart, scriptPart);
        Order();
        sourceRefGo = null;
    }

    void OrderSibling(HierarchyItem item, ref int index)
    {
        if(item.IsNull())
            return;
        item.transform.SetSiblingIndex(++index);
        var children = GetChildren(item.GetID());
        foreach (var child in children)
        {
            OrderSibling(child, ref index);
        }
        children = null;
    }

    void OnHierarchyItemDragEnd(object sender, Vector3 dragEndPosition)
    {
        var draggedItem = sender as HierarchyItem;
        if(draggedItem.reference.IsNull())
            return;
        var itemEntered = GetHierarchyItemEntered();
        if (itemEntered.IsNull())
        {
            var draggedItemRef = draggedItem.reference;
            var prefabricated = prefabManager.GetPrefabricated(draggedItemRef.gameObject);
            if (prefabricated.IsNotNull())
            {
                if (prefabricated.isPrefab)
                {
                    var worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var worldMousePosition2D = worldMousePosition.ToVector2();
                    prefabManager.Unprefab(draggedItemRef, worldMousePosition2D);
                    UpdatePixelPart();
                }
            }
            else
            {
                if (draggedItem.originalParentId == scriptPart.id)
                {
                    var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null)
                    {
                        // just get pixel object
                        var pixel = hit.transform.GetComponent<Pixel>();
                        if (pixel.IsNotNull())
                        {
                            var group = pixel.GetFirstGroup();
                            if(group.IsNotNull())
                            {
                                AssignScriptIntoPixel(draggedItemRef, group.gameObject);
                            }
                            else
                            {
                                AssignScriptIntoPixel(draggedItemRef, pixel.gameObject);
                            }
                        }
                    }
                }
            }
            prefabricated = null;
            draggedItemRef = null;
        }
        else
        {
            DragItemIntoItem(draggedItem, itemEntered);
        }
        draggedItem = null;
        itemEntered = null;
    }

    void DragItemIntoItem(HierarchyItem source, HierarchyItem destination)
    {
        var sourceRef = source.reference;
        if (sourceRef.IsNull())
            return;

        var destId = destination.id;
        if (destId == prefabPart.id)
        {
            // destination is Prefab part
            // source must be a pixel type
            CreatePrefab(source.reference);
        }
        else if (destId == scriptPart.id)
        {
            // destination is Script part
            // have no idea
            
        }
        // else if (destId == pixelPart.id)
        // {
        //     // destination Pixel part
        //     // duplicate of any prefab type
        //     CreatePixelThroughPrefab(source, destination);
        //     UpdatePixelPart();
        //     // or sorting maybe
        // }
        else
        {
            // var parentDestId = destination.originalParentId;
            // if(parentDestId == pixelPart.id)
            // {
            //     AssignScriptIntoPixel(source.reference, destination.reference);
            // }
        }
        sourceRef = null;
    }

    void AssignScriptIntoPixel(GameObject sourceRef, GameObject destRef)
    {
        if (sourceRef.IsNull() || destRef.IsNull())
            return;
        var scriptable = sourceRef.GetComponent<Scriptable>();
        var host = destRef.GetComponent<ScriptableHost>();
        if (scriptable.IsNull() || host.IsNull())
        {
            sourceRef = null;
            destRef = null;
            return;
        }
        host.AddScript(scriptable);
        scriptable = null;
        host = null;
    }

    void CreatePixelThroughPrefab(HierarchyItem source, HierarchyItem destination)
    {
        var sourceRef = source.reference;
        if (sourceRef.IsNull())
            return;
        var sourceRefGo = sourceRef.gameObject;
        var prefabricated = prefabManager.GetPrefabricated(sourceRefGo);
        if (!prefabricated.isPrefab)
        {
            prefabricated = null;
            sourceRefGo = null;
            return;
        }
        var unprefabGoRef = prefabManager.Unprefab(sourceRefGo);
        var unprefabName = unprefabGoRef.name;
        UpdatePixelPart();
        unprefabGoRef = null;
        sourceRefGo = null;
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

    IEnumerator UpdateScriptName()
    {
        while(gameObject.IsNotNull())
        {
            var itemChildOfScriptPart = items.Where(x => x.originalParentId == scriptPart.id).ToList();
            foreach(var item in itemChildOfScriptPart)
            {
                var itemRef = item.reference;
                if(itemRef.IsNotNull()){
                    item.text.text = itemRef.name;
                }
            }
            itemChildOfScriptPart.Clear();
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

    public void UpdatePixelPart()
    {
        // ClearPixelPart();
        // var groups = FindObjectsOfType<Group>();
        // var hierarchyitems = _items.Where(x => x.reference.IsNotNull());
        // // create group items
        // foreach (var group in groups)
        // {
        //     var groupName = group.name;
        //     Create(groupName, groupName, true, group.gameObject, pixelPart, pixelPart);
        //     groupName = null;
        // }
        // // nested groups
        // var groupHasParent = groups.Where(x => x.transform.parent.IsNotNull()).ToArray();
        // foreach(var group in groupHasParent)
        // {
        //     var parentGroupItem = hierarchyitems.FirstOrDefault(x => x.reference.GetID() == group.transform.parent.GetID());
        //     if(parentGroupItem.IsNull())
        //         continue;
        //     var groupItem = hierarchyitems.FirstOrDefault(x => x.reference.GetID() == group.id);
        //     if(groupItem.IsNull())
        //         continue;
        //     groupItem.SetParent(parentGroupItem);
        //     parentGroupItem = null;
        //     groupItem = null;
        // }
        // // pixels in each group
        // foreach (var group in groups)
        // {
        //     var groupId = group.GetID();
        //     var parentGroupItem = hierarchyitems.FirstOrDefault(x => x.reference.GetID() == groupId);
        //     if(parentGroupItem.IsNull())
        //         continue;
        //     var belongPixels = group.GetComponentsInChildren<Pixel>();
        //     belongPixels = belongPixels.Where(x => x.parent.GetID() == groupId).ToArray();
        //     foreach (var pixel in belongPixels)
        //     {
        //         var pixelName = pixel.name;
        //         Create(pixelName, pixelName, true, pixel.gameObject, parentGroupItem, pixelPart);
        //         pixelName = null;
        //     }
        //     belongPixels = null;
        // }
        // // pixels without group
        // var pixels = FindObjectsOfType<Pixel>();
        // var nonGroupPixels = pixels.Where(x => !Group.x.group.IsNotNull()).ToArray();
        // foreach(var pixel in nonGroupPixels)
        // {
        //     var pixelName = pixel.name;
        //     Create(pixelName, pixelName, true, pixel.gameObject, pixelPart, pixelPart);
        //     pixelName = null;
        // }
        // groups = null;
        // hierarchyitems = null;

        // Order();
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

    void ClearPixelPart()
    {
        var pixelPartItems = _items.Where(x => x.originalParentId != 0 && x.originalParentId == pixelPart.id).ToArray();
        _items.RemoveAll(x => pixelPartItems.Any(x1 => x1.id == x.id));
        foreach(var pixelItem in pixelPartItems)
        {
            DestroyImmediate(pixelItem.gameObject);
        }
        pixelPartItems = null;
    }

    public void ClearPixelPart(int itemId)
    {
        var removedItem = _items.FirstOrDefault(x => x.originalParentId != 0 && x.originalParentId == pixelPart.id && x.id == itemId);
        _items.RemoveAll(x => x.id == removedItem.id);
        DestroyImmediate(removedItem.gameObject);
        removedItem = null;
    }

    void ClearPrefabPart()
    {
        var pixelPartItems = _items.Where(x => x.originalParentId != 0 && x.originalParentId == prefabPart.id).ToArray();
        _items.RemoveAll(x => pixelPartItems.Any(x1 => x1.id == x.id));
        foreach(var pixelItem in pixelPartItems)
        {
            DestroyImmediate(pixelItem.gameObject);
        }
        pixelPartItems = null;
    }

    public void ClearPrefabPart(int itemId)
    {
        var removedItem = _items.FirstOrDefault(x => x.originalParentId != 0 && x.originalParentId == prefabPart.id && x.id == itemId);
        _items.RemoveAll(x => x.id == removedItem.id);
        DestroyImmediate(removedItem.gameObject);
        removedItem = null;
    }

    public void ClearScriptPart(int itemId)
    {
        var removedItem = _items.FirstOrDefault(x => x.originalParentId != 0 && x.originalParentId == scriptPart.id && x.id == itemId);
        _items.RemoveAll(x => x.id == removedItem.id);
        DestroyImmediate(removedItem.gameObject);
        removedItem = null;
    }
}
