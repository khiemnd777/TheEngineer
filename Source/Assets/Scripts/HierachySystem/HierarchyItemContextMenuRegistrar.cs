using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchyItemContextMenuRegistrar : ContextMenuRegistrar
{
    HierarchyItem item;
    protected override void Start()
    {
        item = GetComponent<HierarchyItem>();
        base.Start();
    }

    public override void Register()
    {
        if(Constants.HIERARCHY_SCRIPT_PART.Equals(item.name)){
            RegisterItem("create-script", "Create script", () =>
            {
                Debug.Log("Script created.");
            });
        }
    }
}
