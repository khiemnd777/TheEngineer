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
    ScriptManager scriptManager;

    void Awake()
    {
        scriptManager = ScriptManager.instance;
        hierarchyManager = HierarchyManager.instance;
    }

    protected override void Start()
    {
        item = GetComponent<HierarchyItem>();
        base.Start();
    }

    public override void Register()
    {
        if (Constants.HIERARCHY_SCRIPT_PART.Equals(item.name))
        {
            RegisterItem("create-script", "Create script", () =>
            {
                Debug.Log("Script created.");
                var scriptable = Scriptable.CreateInstance();
                hierarchyManager.CreateScript(scriptable.gameObject);
                scriptManager.ShowUnimplementedScriptPanel(scriptable);
            });
        }
        if (!(Constants.HIERARCHY_PIXEL_PART.Equals(item.name)
            || Constants.HIERARCHY_PREFAB_PART.Equals(item.name)
            || Constants.HIERARCHY_SCRIPT_PART.Equals(item.name)))
        {
            if(item.originalParentId == hierarchyManager.scriptPart.id)
            {
                RegisterItem("edit-script", "Edit Script", () =>
                {
                    Debug.Log("Edit script.");
                    var itemRef = item.reference;
                    if(itemRef.IsNull())
                        return;
                    var scriptable = itemRef.GetComponent<Scriptable>();
                    scriptManager.ShowUnimplementedScriptPanel(scriptable);
                });
            }
            RegisterItem("rename", "Rename", () =>
            {
                Debug.Log("rename.");
                item.ToggleInputField();
            });
            RegisterItem("delete", "Delete", takeOffPrefab, () =>
            {
                if(item.originalParentId == 0)
                    return;
                if (item.originalParentId == hierarchyManager.prefabPart.id)
                {
                    var itemRef = item.reference;
                    if (itemRef.IsNotNull())
                {
                        DestroyImmediate(itemRef.gameObject);
                    }
                    hierarchyManager.ClearPrefabPart(item.id);
                }
                // else if (item.originalParentId == hierarchyManager.pixelPart.id)
                // {
                //     var itemRef = item.reference;
                //     if (itemRef.IsNotNull())
                //     {
                //         var itemRefIsPixel = itemRef.GetComponent<Pixel>();
                //         if(itemRefIsPixel.IsNotNull())
                //         {
                //             itemRefIsPixel.Remove();
                //         }
                //         else
                //         {
                //             var itemRefIsGroup = itemRef.GetComponent<Group>();
                //             if(itemRefIsGroup.IsNotNull())
                //                 itemRefIsGroup.Remove();
                //         }
                //         hierarchyManager.ClearPixelPart(item.id);
                //         hierarchyManager.UpdatePixelPart();
                //     }
                // }
                else if(item.originalParentId == hierarchyManager.scriptPart.id)
                {
                    var itemRef = item.reference;
                    if (itemRef.IsNotNull())
                    {
                        var itemRefIsScriptable = itemRef.GetComponent<Scriptable>();
                        if(itemRefIsScriptable.IsNotNull()){
                            itemRefIsScriptable.Remove();
                            hierarchyManager.ClearScriptPart(item.id);
                        }
                    }
                }
            });
        }
    }
}
