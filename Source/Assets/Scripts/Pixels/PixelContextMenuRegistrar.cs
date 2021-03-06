using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public class PixelContextMenuRegistrar : ContextMenuRegistrar
{
    public Button scriptItPrefab;
    public Button takeOffPrefab;

    Pixel pixel;

    HierarchyManager hierarchyManager;
    ScriptManager scriptManager;
    List<Pixel> _pixels;

    protected override void Start()
    {
        base.Start();
        hierarchyManager = HierarchyManager.instance;
        scriptManager = ScriptManager.instance;
        pixel = GetComponent<Pixel>();
    }

    public override void Register()
    {
        RegisterItem("add-script", "Add Script", scriptItPrefab, () =>
        {
            Debug.Log("Script added");
            var host = pixel.GetScriptableHost();
            var scriptable = Scriptable.CreateInstanceAndAssignTo(host);
            hierarchyManager.CreateScript(scriptable.gameObject);
            scriptManager.ShowImplementedScriptPanel(host);
        });

        RegisterItem("edit-script", "Edit Script", scriptItPrefab, () =>
        {
            Debug.Log("Edit script");
            var host = !pixel.group.IsNotNull()
                ? GetComponent<ScriptableHost>()
                : pixel.GetFirstGroup().GetComponent<ScriptableHost>();
            scriptManager.ShowImplementedScriptPanel(host);
        });

        RegisterItem("toggle-pivot", "Toggle Pivot", () =>
        {
            Debug.Log("Toggle pivot");
            if(pixel.group.IsNotNull())
            {
                var firstGroup = pixel.GetFirstGroup();
                firstGroup.TogglePivot();
            }
            else
            {
                pixel.TogglePivot();
            }
        });

        RegisterItem("group", "Group", () =>
        {
            Debug.Log("Group");
            Group.Create();
            hierarchyManager.UpdatePixelPart();
        });

        RegisterItem("ungroup-one-by-one", "Ungroup", () =>
        {
            Debug.Log("Ungroup One-by-one");
            Group.UngroupOneByOne();
            hierarchyManager.UpdatePixelPart();
        });

        RegisterItem("ungroup-single", "Ungroup", () =>
        {
            Debug.Log("Ungroup Single");
            Group.UngroupSingle(pixel);
            hierarchyManager.UpdatePixelPart();
        });

        RegisterItem("ungroup-all", "Ungroup All", () =>
        {
            Debug.Log("Ungroup All");
            Group.Ungroup();
            hierarchyManager.UpdatePixelPart();
        });

        RegisterItem("create-prefab", "Create Prefab", () =>
        {
            Debug.Log("Create Prefab");
            // var pixels = FindObjectsOfType<Pixel>();
            // var selectedPixels = pixels.Where(x => x.selecting);
            var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting);
            if (selectedPixels.Any())
            {
                var groupsAlreadyInPrefab = new List<Group>();
                var selectedPixelsToArray = selectedPixels.ToArray();
                foreach (var pixel in selectedPixelsToArray)
                {
                    var group = pixel.GetFirstGroup();
                    if (group.IsNotNull())
                    {
                        if (groupsAlreadyInPrefab.Any(x => x.id == group.id))
                            continue;
                        groupsAlreadyInPrefab.Add(group);
                        hierarchyManager.CreatePrefab(group.gameObject);
                        continue;
                    }
                    hierarchyManager.CreatePrefab(pixel.gameObject);
                }
                groupsAlreadyInPrefab.Clear();
                groupsAlreadyInPrefab = null;
                selectedPixelsToArray = null;
                selectedPixels = null;
            }
            else
            {
                var group = pixel.GetFirstGroup();
                if (group.IsNotNull())
                {
                    hierarchyManager.CreatePrefab(group.gameObject);
                    return;
                }
                hierarchyManager.CreatePrefab(pixel.gameObject);
            }
        });

        RegisterItem("take-off", "Delete", takeOffPrefab, () =>
        {
            Debug.Log("Take Off");
            // PixelRemovingManager.instance.Remove();
            var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting);
            if(selectedPixels.Any())
            {
                var potentialDeletingGroups = Group.GetManyGroups(selectedPixels);
                var it = selectedPixels.ToList();
                it.ForEach(x => x.Remove());
                var potentialDeletingGroupsIt = potentialDeletingGroups.Where(x=>x.IsNotNull()).ToList();
                Debug.Log(potentialDeletingGroupsIt.Count);
                potentialDeletingGroupsIt.ForEach(group => group.Remove());
                potentialDeletingGroupsIt.Clear();
                potentialDeletingGroups = null;
            }
            else
            {
                pixel.Remove();
            }
            hierarchyManager.UpdatePixelPart();
        });
    }

    public override void OnBeforeShow()
    {
        base.OnBeforeShow();
        // var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting);
        var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting);
        if (selectedPixels.Any())
        {
            var numberOfSelectedGroup = Group.GetManyGroups(selectedPixels);
            var numberOfPixelWithoutGroup = selectedPixels.Count(x => !x.group.IsNotNull());
            shownItems = menuItems.Where(x =>
                // || pixel.group.IsNotNull() && x.Key == "add-script"
                !pixel.HasScript() && !pixel.group.IsNotNull() && numberOfPixelWithoutGroup == 1 && x.Key == "add-script"
                || !pixel.HasScript() && pixel.group.IsNotNull() && x.Key == "add-script"
                || pixel.HasScript() && x.Key == "edit-script"
                || x.Key == "toggle-pivot"
                || x.Key == "create-prefab"
                || numberOfPixelWithoutGroup > 1 && x.Key == "group"
                || numberOfSelectedGroup.Count > 1 && x.Key == "group"
                || numberOfSelectedGroup.Count >= 1 && numberOfPixelWithoutGroup >= 1 && x.Key == "group"
                || pixel.group.IsNotNull() && numberOfSelectedGroup.Count == 1 && x.Key == "ungroup-one-by-one"
                || pixel.group.IsNotNull() && numberOfSelectedGroup.Count == 1 && x.Key == "ungroup-all"
                || x.Key == "take-off")
            .ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            shownItems = menuItems.Where(x =>
                !pixel.HasScript() && !pixel.group.IsNotNull() && x.Key == "add-script"
                || pixel.HasScript() && x.Key == "edit-script"
                || x.Key == "toggle-pivot"
                || x.Key == "create-prefab"
                || pixel.group.IsNotNull() && x.Key == "ungroup-single"
                || x.Key == "take-off")
            .ToDictionary(x => x.Key, x => x.Value);
        }
        selectedPixels = null;
    }
}