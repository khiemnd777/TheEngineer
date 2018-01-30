using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchyItemContextMenuRegistrar : ContextMenuRegistrar
{
    public Button takeOffPrefab;

    HierarchyItem item;
    HierarchyManager hierarchyManager;

    protected override void Start()
    {
        item = GetComponent<HierarchyItem>();
        hierarchyManager = HierarchyManager.instance;
        base.Start();
    }

    public override void Register()
    {
        if (Constants.HIERARCHY_SCRIPT_PART.Equals(item.name))
        {
            RegisterItem("create-script", "Create script", () =>
            {
                Debug.Log("Script created.");
            });
        }
        if (!(Constants.HIERARCHY_PIXEL_PART.Equals(item.name)
            || Constants.HIERARCHY_PREFAB_PART.Equals(item.name)
            || Constants.HIERARCHY_SCRIPT_PART.Equals(item.name)))
        {
            RegisterItem("rename", "Rename", () =>
            {
                Debug.Log("rename.");
            });
            RegisterItem("delete", "Delete", takeOffPrefab, () =>
            {
                if (item.originalParentId != 0 && item.originalParentId == hierarchyManager.prefabPart.id)
                {
                    var itemRef = item.reference;
                    if (itemRef.IsNotNull())
                    {
                        DestroyImmediate(itemRef.gameObject);
                    }
                    hierarchyManager.ClearPrefabPart(item.id);
                }
                else if (item.originalParentId != 0 && item.originalParentId == hierarchyManager.pixelPart.id)
                {
                    var itemRef = item.reference;
                    if (itemRef.IsNotNull())
                    {
                        DestroyImmediate(itemRef.gameObject);
                    }
                    hierarchyManager.ClearPixelPart(item.id);
                }
            });
        }
    }
}
