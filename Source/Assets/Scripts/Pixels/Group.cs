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
            var groupPosition = centerPoint.ToVector2().Snap2();
            var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
            var group = Instantiate<Group>(groupPrefab, groupPosition, Quaternion.identity);
            var pivot = group.GetComponentInChildren<GroupPivot>();
            pivot.transform.position = new Vector3(centerPoint.x, centerPoint.y, pivot.transform.position.z);
            // set parent for each selected pixel those are without any group
            var selectedPixelsWithoutGroup = selectedPixels.Where(x => !HasGroup(x)).ToList();
            foreach (var pixel in selectedPixelsWithoutGroup)
            {
                // pixel.grouping = true;
                pixel.transform.SetParent(group.transform);
            }
            // set parent for each group what has any selected pixel
            var selectedGroups = GetManyGroups(selectedPixels);
            foreach (var selectedGroup in selectedGroups)
            {
                // disabling pivot of selected group before set parent for it
                var pivotOfSelectedGroup = selectedGroup.GetComponentInChildren<GroupPivot>();
                pivotOfSelectedGroup.GetComponent<MeshRenderer>().enabled = false;
                selectedGroup.transform.SetParent(group.transform);
            }

            selectedGroups = null;
            selectedPoints = null;
            group = null;
            groupPrefab = null;
        }
    }

    public static void UngroupOneByOne()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting && HasGroup(x));
        if(!selectedPixels.Any())
            return;
        var firstGroup = GetFirstGroup(selectedPixels.LastOrDefault());
        foreach(var pixel in selectedPixels){
            var lastGroup = GetLastGroup(pixel);
            var parentGroupsOfLastGroup = lastGroup.GetComponentsInParent<Group>();
            var nearestParentGroupOfLastGroup = parentGroupsOfLastGroup[parentGroupsOfLastGroup.Length - 1];
            if(nearestParentGroupOfLastGroup.transform.GetInstanceID() == firstGroup.transform.GetInstanceID())
            {
                if(parentGroupsOfLastGroup.Length > 1){
                    nearestParentGroupOfLastGroup = parentGroupsOfLastGroup[parentGroupsOfLastGroup.Length - 2];
                    nearestParentGroupOfLastGroup.transform.parent = null;
                }
            }
            
            if(firstGroup.transform.GetInstanceID() == lastGroup.transform.GetInstanceID()){
                pixel.transform.parent = null;
            }
            lastGroup = null;
            parentGroupsOfLastGroup = null;
            nearestParentGroupOfLastGroup = null;
        }
        selectedPixels = null;
        pixels = null;
        Destroy(firstGroup.gameObject);
    }

    public static void Ungroup()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting && HasGroup(x));
        if (!selectedPixels.Any())
            return;
        foreach (var pixel in selectedPixels)
        {
            UngroupSingle(pixel);
        }
    }

    public static void UngroupSingle(Pixel pixel)
    {
        var groupOfPixel = GetFirstGroup(pixel);
        pixel.transform.parent = null;
        var pixelsInGroup = groupOfPixel.GetComponentsInChildren<Pixel>();
        if (pixelsInGroup.Length <= 1)
        {
            if(pixelsInGroup.Length == 1){
                pixelsInGroup[0].transform.parent = null;
            }
            Destroy(groupOfPixel.gameObject);
        }
        groupOfPixel = null;
        pixelsInGroup = null;
    }

    public static Group GetFirstGroup(Pixel pixel)
    {
        var groupsOfPixel = GetGroups(pixel);
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel[groupsOfPixel.Length - 1];
    }

    public static Group GetLastGroup(Pixel pixel)
    {
        var groupsOfPixel = GetGroups(pixel);
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel[0];
    }

    public static Group GetGroupAtIndex(Pixel pixel, int index)
    {
        var groupsOfPixel = GetGroups(pixel);
        if(!groupsOfPixel.Any())
            return null;
        return groupsOfPixel[groupsOfPixel.Length - 1 - index];
    }

    public static Group[] GetGroups(Pixel pixel)
    {
        var groupsOfPixel = pixel.GetComponentsInParent<Group>();
        return groupsOfPixel;
    }

    public static List<Group> GetManyGroups(IEnumerable<Pixel> pixels)
    {
        var groups = new List<Group>();
        foreach (var pixel in pixels)
        {
            if (!HasGroup(pixel))
                continue;
            var group = GetFirstGroup(pixel);
            if (groups.Count > 0 && groups.Any(x => x.transform.GetInstanceID() == group.transform.GetInstanceID()))
            {
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
        var groupOfPixel = GetFirstGroup(pixel);
        var result = groupOfPixel.IsNotNull();
        groupOfPixel = null;
        return result;
    }

    public static Pixel[] GetPixelsInGroupByPixel(Pixel pixel)
    {
        var groupOfPixel = GetFirstGroup(pixel);
        var pixelsInGroup = groupOfPixel.GetComponentsInChildren<Pixel>();
        groupOfPixel = null;
        return pixelsInGroup;
    }

    public static void DeselectAnotherPixelsByPixel(Pixel pixel){
        var pixels = FindObjectsOfType<Pixel>();
        var pixelsHasSelectedPixel = GetPixelsInGroupByPixel(pixel);
        var selectedPixels = pixels.Where(x => x.selecting && !pixelsHasSelectedPixel.Any(y => y.id == x.id)).ToList();
        foreach(var selectedPixel in selectedPixels){
            selectedPixel.Deselect();
        }
        selectedPixels = null;
        pixelsHasSelectedPixel = null;
        pixels = null;
    }

    public static void SelectPixelsInGroupFollowSelectedPixel()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var pixelsHasGroup = pixels.Where(x => HasGroup(x)).ToList();
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
            pixelsInGroup = pixelsInGroup.Where(x => !x.selecting && HasGroup(x)).ToArray();
            foreach (var pixelInGroup in pixelsInGroup)
            {
                pixelInGroup.Select();
            }
            pixelsInGroup = null;
        }
        // active the pivot of group
        var manySelectedGroups = GetManyGroups(pixelsHasGroup);
        foreach(var group in manySelectedGroups){
            var pivot = group.GetComponentInChildren<GroupPivot>();
            if(pivot.IsNull())
                continue;
            var pixel = group.GetComponentInChildren<Pixel>();
            if(pixel.IsNotNull())
            {
                pivot.GetComponent<MeshRenderer>().enabled = pixel.selecting;
                pixel = null;
            }
            pivot = null;
            
        }
        manySelectedGroups = null;
        pixelsHasGroup = null;
        pixelsHasGroupButWithoutSelected = null;
        pixels = null;
    }
}