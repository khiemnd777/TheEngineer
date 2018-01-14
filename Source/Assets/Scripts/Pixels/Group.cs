using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour
{
    public Pivot pivot;

    public static void Create()
    {
        var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting && !x.grouping);
        var selectedPixels = pixels.Where(x => x.selecting);
        if (selectedPixels.Any())
        {
            var selectedPoints = selectedPixels.Select(x => x.transform.position).ToArray();
            var centerPoint = TransformUtility.ComputeCenterPoint(selectedPoints);
            var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
            var group = Instantiate<Group>(groupPrefab, centerPoint, Quaternion.identity);
            // set parent for each selected pixel those are without any group
            var selectedPixelsWithoutGroup = selectedPixels.Where(x => !HasGroup(x)).ToList();
            foreach (var pixel in selectedPixelsWithoutGroup)
            {
                // pixel.grouping = true;
                pixel.transform.SetParent(group.transform);
            }
            // set parent for each group what has any selected pixel
            var selectedGroups = GetManyGroups(selectedPixels);
            foreach(var selectedGroup in selectedGroups){
                selectedGroup.transform.SetParent(group.transform);
            }

            selectedGroups = null;
            selectedPoints = null;
            group = null;
            groupPrefab = null;
        }
    }

    public static void Ungroup()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting && x.grouping);
        if (selectedPixels.Any())
        {
            foreach (var pixel in selectedPixels)
            {
                UngroupSingle(pixel);
            }
        }
    }

    public static void UngroupSingle(Pixel pixel)
    {
        var groupOfPixel = GetGroup(pixel);
        pixel.grouping = false;
        pixel.transform.parent = null;
        var pixelsInGroup = groupOfPixel.GetComponentsInChildren<Pixel>();
        if (pixelsInGroup.Length == 0)
        {
            Destroy(groupOfPixel.gameObject);
        }
        groupOfPixel = null;
        pixelsInGroup = null;
    }

    public static Group GetGroup(Pixel pixel)
    {
        var groupOfPixel = pixel.GetComponentInParent<Group>();
        return groupOfPixel;
    }

    public static List<Group> GetManyGroups(IEnumerable<Pixel> pixels){
        var groups = new List<Group>();
        foreach(var pixel in pixels){
            if(!HasGroup(pixel))
                continue;
            var group = GetGroup(pixel);
            if(groups.Count > 0 && groups.Any(x => x.transform.GetInstanceID() == group.transform.GetInstanceID())){
                group = null;
                continue;
            }
            groups.Add(group);
            group = null;
        }
        return groups;
    }

    public static bool HasGroup(Pixel pixel)
    {
        var groupOfPixel = GetGroup(pixel);
        var result = groupOfPixel.IsNotNull();
        groupOfPixel = null;
        return result;
    }

    public static Pixel[] GetPixelsInGroupByPixel(Pixel pixel)
    {
        var groupOfPixel = GetGroup(pixel);
        var pixelsInGroup = groupOfPixel.GetComponentsInChildren<Pixel>();
        groupOfPixel = null;
        return pixelsInGroup;
    }

    public static void SelectPixelsInGroupFollowSelectedPixel()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var pixelsHasGroup = pixels.Where(x => x.grouping).ToList();
        var pixelsHasGroupButWithoutSelected = new List<Pixel>();
        foreach (var pixel in pixelsHasGroup)
        {
            if (pixelsHasGroupButWithoutSelected.Any(x => x.id == pixel.id))
                continue;
            if (!pixel.selecting)
            {
                pixelsHasGroupButWithoutSelected.Add(pixel);
                continue;
            }
            var pixelsInGroup = Group.GetPixelsInGroupByPixel(pixel);
            pixelsInGroup = pixelsInGroup.Where(x => !x.selecting && x.grouping).ToArray();
            foreach (var pixelInGroup in pixelsInGroup)
            {
                pixelInGroup.Select();
            }
            pixelsInGroup = null;
        }
        pixelsHasGroup = null;
        pixelsHasGroupButWithoutSelected = null;
    }
}