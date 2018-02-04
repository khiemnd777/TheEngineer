using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour, IPrefabricated
{
    public Pivot pivot;
    public int id;

    public bool isPrefab { get; set; }

    List<Pixel> _pixels;

    public ICollection<Pixel> pixels
    {
        get 
        {
            return _pixels ?? (_pixels = new List<Pixel>());
        }
    }

    List<Group> _groupChildren;

    public ICollection<Group> groupChildren
    {
        get 
        {
            return _groupChildren ?? (_groupChildren = new List<Group>());
        }
    }

    Group _parentalGroup;

    public Group parentalGroup
    {
        get { return _parentalGroup; }
    }

    void Start()
    {
        id = this.GetID();        
    }

    public void AddPixel(Pixel pixel)
    {
        pixels.Add(pixel);
        pixel.AddToGroup(this);
    }

    public void AddChildGroup(Group group)
    {
        group.AssignParentGroup(this);
        groupChildren.Add(group);
    }

    public void AssignParentGroup(Group group)
    {
        _parentalGroup = group;
        transform.SetParent(group.transform);
    }

    public void UnassignParentalGroup()
    {
        _parentalGroup = null;
        transform.parent = null;
    }

    public static void Create()
    {
        // var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting);
        var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting);
        if (selectedPixels.Any())
        {
            Create(selectedPixels);
        }
    }

    public static Group Create(IEnumerable<Pixel> pixels)
    {
        var selectedPoints = pixels.Select(x => x.transform.position).ToArray();
        var centerPoint = TransformUtility.ComputeCenterPoint(selectedPoints);
        var groupPosition = centerPoint.ToVector2().Snap2();
        var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
        var group = Instantiate<Group>(groupPrefab, groupPosition, Quaternion.identity);
        var pivot = group.GetComponentInChildren<GroupPivot>();
        pivot.transform.position = new Vector3(centerPoint.x, centerPoint.y, pivot.transform.position.z);
        // set parent for each selected pixel those are without any group
        var selectedPixelsWithoutGroup = pixels.Where(x => x.group.IsNull()).ToList();
        foreach (var pixel in selectedPixelsWithoutGroup)
        {
            group.AddPixel(pixel);
        }
        // set parent for each group what has any selected pixel
        var selectedGroups = GetManyGroups(pixels);
        foreach (var selectedGroup in selectedGroups)
        {
            if(selectedGroup.id == group.id)
                continue;
            // disabling pivot of selected group before set parent for it
            var pivotOfSelectedGroup = selectedGroup.GetComponentInChildren<GroupPivot>();
            pivotOfSelectedGroup.GetComponent<MeshRenderer>().enabled = false;
            group.AddChildGroup(selectedGroup);
        }

        selectedGroups = null;
        selectedPoints = null;
        groupPrefab = null;
        return group;
    }

    public static void UngroupOneByOne()
    {
        // var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting && x.group.IsNotNull());
        var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting && x.group.IsNotNull());
        if (!selectedPixels.Any())
            return;
        var firstGroup = GetFirstGroup(selectedPixels.Last());
        var selectPixelsToArray = selectedPixels.ToArray();
        foreach (var pixel in selectPixelsToArray)
        {
            // var parentGroupsOfLastGroup = lastGroup.GetComponentsInParent<Group>();
            var parentGroups = GetGroups(pixel);
            var nearestParentGroupOfLastGroup = parentGroups.Last();
            if (nearestParentGroupOfLastGroup.id == firstGroup.id)
            {
                var countOfParentGroups = parentGroups.Count();
                if (countOfParentGroups > 1)
                {
                    nearestParentGroupOfLastGroup = parentGroups.ElementAt(countOfParentGroups - 2);
                    nearestParentGroupOfLastGroup.UnassignParentalGroup();
                }
            }
            var lastGroup = parentGroups.First();
            if (lastGroup.IsNotNull() && firstGroup.id == lastGroup.id)
            {
                pixel.RemoveGroup();
            }
            nearestParentGroupOfLastGroup = null;
        }
        selectedPixels = null;
        selectPixelsToArray = null;
        firstGroup.Remove();
    }

    public static void Ungroup()
    {
        // var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting && x.group.IsNotNull());
        var selectedPixels = PixelManager.instance.GetPixels(x=>x.selecting && x.group.IsNotNull());
        if (!selectedPixels.Any())
            return;
        var selectedPixelsToArray = selectedPixels.ToArray();
        foreach (var pixel in selectedPixelsToArray)
        {
            UngroupSingle(pixel);
        }
    }

    public static void UngroupSingle(Pixel pixel)
    {
        var groupOfPixel = GetFirstGroup(pixel);
        pixel.RemoveGroup();
        UngroupIfHasOnePixel(groupOfPixel);
        groupOfPixel = null;
    }

    public static void UngroupIfHasOnePixel(Group group)
    {
        var pixelsInGroup = group.pixels;
        var countOfPixels = pixelsInGroup.Count;
        if (countOfPixels <= 1)
        {
            if (countOfPixels == 1)
            {
                var firstPixelInGroup = pixelsInGroup.First();
                if(firstPixelInGroup.IsNotNull())
                {
                    firstPixelInGroup.RemoveGroup();
                }
            }
            group.Remove();
        }
        pixelsInGroup = null;
    }

    public static Group GetFirstGroup(Pixel pixel)
    {
        var groupsOfPixel = GetGroups(pixel);
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.Last();
    }

    public static Group GetLastGroup(Pixel pixel)
    {
        var groupsOfPixel = GetGroups(pixel);
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.First();
    }

    public static Group GetGroupAtIndex(Pixel pixel, int index)
    {
        var groupsOfPixel = GetGroups(pixel);
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.ElementAt(groupsOfPixel.Count() - 1 - index);
    }

    public static IEnumerable<Group> GetGroups(Pixel pixel)
    {
        var groupOfPixel = pixel.group;
        var parentalGroups = new List<Group>();
        if(groupOfPixel.IsNull())
            return parentalGroups;
        parentalGroups.Add(groupOfPixel);
        GetParentalGroups(groupOfPixel, ref parentalGroups);
        return parentalGroups;
    }

    public static void GetParentalGroups(Group group, ref List<Group> parentalGroups){
        var outsideGroup = group.parentalGroup;
        if(outsideGroup.IsNotNull())
        {
            parentalGroups.Add(outsideGroup);
            GetParentalGroups(outsideGroup, ref parentalGroups);
        }
    }

    public static List<Group> GetManyGroups(IEnumerable<Pixel> pixels)
    {
        var groups = new List<Group>();
        foreach (var pixel in pixels)
        {
            if (pixel.group.IsNull())
                continue;
            var group = GetFirstGroup(pixel);
            if (groups.Count > 0 && groups.Any(x => x.id == group.id))
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

    public static IEnumerable<Pixel> GetPixelsInGroupByPixel(Pixel pixel)
    {
        var groupOfPixel = GetFirstGroup(pixel);
        var pixelsInGroup = groupOfPixel.GetPixelsInChildren();
        return pixelsInGroup;
    }

    public static void DeselectAnotherPixelsByPixel(Pixel pixel)
    {
        var pixelsHasSelectedPixel = GetPixelsInGroupByPixel(pixel);
        // var pixels = FindObjectsOfType<Pixel>();
        // var selectedPixels = pixels.Where(x => x.selecting && !pixelsHasSelectedPixel.Any(y => y.GetID() == x.GetID())).ToList();
        var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting && !pixelsHasSelectedPixel.Any(y => y.id == x.id)).ToArray();
        foreach (var selectedPixel in selectedPixels)
        {
            selectedPixel.Deselect();
        }
        selectedPixels = null;
        pixelsHasSelectedPixel = null;
    }

    public static void SelectPixelsInGroupFollowSelectedPixel()
    {
        // var pixels = FindObjectsOfType<Pixel>();
        // var pixelsHasGroup = pixels.Where(x => x.group.IsNotNull()).ToList();
        var pixelsHasGroup = PixelManager.instance.GetPixels(x => x.group.IsNotNull()).ToArray();
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
            var pixelsInGroup = GetPixelsInGroupByPixel(pixel);
            pixelsInGroup = pixelsInGroup.Where(x => !x.selecting && x.group.IsNotNull()).ToArray();
            foreach (var pixelInGroup in pixelsInGroup)
            {
                pixelInGroup.Select();
            }
            pixelsInGroup = null;
        }
        // active the pivot of group
        var manySelectedGroups = GetManyGroups(pixelsHasGroup);
        foreach (var group in manySelectedGroups)
        {
            var pivot = group.GetComponentInChildren<GroupPivot>();
            if (pivot.IsNull())
                continue;
            var pixel = group.pixels.First();
            if (pixel.IsNotNull())
            {
                pivot.GetComponent<MeshRenderer>().enabled = pixel.selecting;
                pixel = null;
            }
            pivot = null;

        }
        manySelectedGroups = null;
        pixelsHasGroup = null;
        pixelsHasGroupButWithoutSelected = null;
    }

    public void Remove()
    {
        var pixels = GetPixelsInChildren();
        foreach(var pixel in pixels)
        {
            var scriptHost = pixel.GetComponent<ScriptableHost>();
            scriptHost.RemoveAllScript();
        }
        foreach(var group in groupChildren)
        {
            var scriptHost = group.GetComponent<ScriptableHost>();
            scriptHost.RemoveAllScript();
        }
        var scriptHostOfGroup = GetComponent<ScriptableHost>();
        if(scriptHostOfGroup.IsNotNull())
        {
            scriptHostOfGroup.RemoveAllScript();
        }
        _pixels = null;
        _groupChildren = null;
        DestroyImmediate(gameObject);
    }

    public IEnumerable<Pixel> GetPixelsInChildren()
    {
        var list = new List<Pixel>();
        list.AddRange(pixels);
        foreach(var group in groupChildren)
        {
            list.AddRange(group.pixels);
        }
        return list;
    }

    public GameObject Prefabricate(GameObject patternObject, Transform prefabContainer)
    {
        var groupPrefab = Resources.Load<GameObject>(Constants.GROUP_PREFAB);
        var prefabGo = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity, prefabContainer);
        var group = prefabGo.GetComponent<Group>();
        if (group.IsNotNull())
        {
            var patternGroup = patternObject.GetComponent<Group>();
            var groups = patternGroup.groupChildren;
            var pixelsInGroup = patternGroup.pixels;
            foreach (var pixel in pixelsInGroup)
            {
                // PixelManager.instance.AddPixel(pixel);
                var pixelTransform = pixel.transform;
                var instancePixel = Instantiate(pixel, pixelTransform.position, pixelTransform.rotation);
                instancePixel.name = pixel.name;
                group.AddPixel(instancePixel);
            }
            group.CloneGroup(groups);
        }
        return prefabGo;
    }

    public GameObject Unprefabricate(GameObject patternObject)
    {
        var groupPrefab = Resources.Load<GameObject>(Constants.GROUP_PREFAB);
        var prefabGo = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity);
        var group = prefabGo.GetComponent<Group>();
        if (group.IsNotNull())
        {
            var patternGroup = patternObject.GetComponent<Group>();
            var groups = patternGroup.groupChildren;
            var pixelsInGroup = patternGroup.pixels;
            foreach (var pixel in pixelsInGroup)
            {
                // PixelManager.instance.AddPixel(pixel);
                var pixelTransform = pixel.transform;
                var instancePixel = Instantiate(pixel, pixelTransform.position, pixelTransform.rotation);
                instancePixel.name = pixel.name;
                group.AddPixel(instancePixel);
                PixelManager.instance.AddPixel(instancePixel);
            }
            group.CloneGroup(groups);
        }
        return prefabGo;
    }

    public void CloneGroup(IEnumerable<Group> cloneGroupChildren)
    {
        foreach(var childGroup in cloneGroupChildren)
        {
            var groupChildren = childGroup.groupChildren;
            if(groupChildren.Any())
            {
                childGroup.CloneGroup(groupChildren);
            }
            var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
            var prefabGo = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity, transform);
            var pixels = childGroup.pixels.Select(x => {
                var xTransform = x.transform;
                var instanceX = Instantiate(x, xTransform.position, xTransform.rotation);
                PixelManager.instance.AddPixel(instanceX);
                instanceX.name = x.name;
                return instanceX;
            }).ToList();
            pixels.ForEach(pixel => prefabGo.AddPixel(pixel));
            AddChildGroup(prefabGo);
            pixels.Clear();
        }
    }
}