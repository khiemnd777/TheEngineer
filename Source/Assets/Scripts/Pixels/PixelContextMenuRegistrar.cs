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

    protected override void Start()
    {
        base.Start();
        pixel = GetComponent<Pixel>();
    }

    public override void Register()
    {
        RegisterItem("script-it", "Script It", scriptItPrefab, () =>
        {
            Debug.Log("Script It");
        });

        RegisterItem("group", "Group", () =>
        {
            Debug.Log("Group");
            Group.Create();
        });

        RegisterItem("ungroup-one-by-one", "Ungroup", () =>
        {
            Debug.Log("Ungroup One-by-one");
            Group.UngroupOneByOne();
        });

        RegisterItem("ungroup-single", "Ungroup", () =>
        {
            Debug.Log("Ungroup Single");
            Group.UngroupSingle(pixel);
        });

        RegisterItem("ungroup-all", "Ungroup All", () =>
        {
            Debug.Log("Ungroup All");
            Group.Ungroup();
        });

        RegisterItem("take-off", "Take Off", takeOffPrefab, () =>
        {
            Debug.Log("Take Off");
        });
    }

    public override void OnBeforeShow()
    {
        base.OnBeforeShow();
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting);
        if (selectedPixels.Any())
        {
            var numberOfSelectedGroup = Group.GetManyGroups(selectedPixels);
            var numberOfPixelWithoutGroup = selectedPixels.Count(x => !Group.HasGroup(x));
            shownItems = menuItems.Where(x => 
                !Group.HasGroup(pixel) && numberOfPixelWithoutGroup == 1 && x.Key == "script-it"
                || numberOfPixelWithoutGroup > 1 && x.Key == "group" 
                || numberOfSelectedGroup.Count > 1 && x.Key == "group" 
                || numberOfSelectedGroup.Count >= 1 && numberOfPixelWithoutGroup >= 1 && x.Key == "group" 
                || Group.HasGroup(pixel) && numberOfSelectedGroup.Count == 1 && x.Key == "ungroup-one-by-one"
                || Group.HasGroup(pixel) && numberOfSelectedGroup.Count == 1 && x.Key == "ungroup-all"
                || x.Key == "take-off")
            .ToDictionary(x => x.Key, x => x.Value); 
        }
        else
        {
            shownItems = menuItems.Where(x => 
                !Group.HasGroup(pixel) && x.Key == "script-it" 
                || Group.HasGroup(pixel) && x.Key == "ungroup-single"
                || x.Key == "take-off")
            .ToDictionary(x => x.Key, x => x.Value);
        }
        pixels = null;
        selectedPixels = null;
    }
}