using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour, IPrefabricated
{
    public int id;
    public GroupPivot pivot;
    public bool isPrefab { get; set; }

    PixelManager pixelManager;

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

    void Awake()
    {
        pixelManager = PixelManager.instance;
        id = this.GetID();
    }

    public void AddPixel(Pixel pixel)
    {
        pixels.Add(pixel);
        pixel.AddToGroup(this);
    }

    public void RemovePixel(Pixel pixel)
    {
        pixels.Remove(pixel);
        pixel.RemoveGroup();
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
        var pivot = group.pivot;
        pivot.transform.position = new Vector3(centerPoint.x, centerPoint.y, pivot.transform.position.z);
        group.SetEnabledPivot(false);
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
            var pivotOfSelectedGroup = selectedGroup.pivot;
            pivotOfSelectedGroup.GetComponent<MeshRenderer>().enabled = false;
            group.AddChildGroup(selectedGroup);
        }
        group.SetEnabledPivot(false);
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
        var lastSelectedPixel = selectedPixels.Last();
        var firstGroup = lastSelectedPixel.GetFirstGroup();
        var selectPixelsToArray = selectedPixels.ToArray();
        foreach (var pixel in selectPixelsToArray)
        {
            // var parentGroupsOfLastGroup = lastGroup.GetComponentsInParent<Group>();
            var parentGroups = pixel.GetGroups();
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
        var groupOfPixel = pixel.group; //GetFirstGroup(pixel);
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

    public static List<Group> GetManyGroups(IEnumerable<Pixel> pixels)
    {
        var groups = new List<Group>();
        foreach (var pixel in pixels)
        {
            if (pixel.group.IsNull())
                continue;
            var group = pixel.GetFirstGroup();
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

    public static void DeselectAnotherPixelsByPixel(Pixel pixel)
    {
        var pixelsHasSelectedPixel = pixel.GetPixelsInGroup();
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
        var pixelsHasGroup = PixelManager.instance.GetPixels(x => x.selecting && x.group.IsNotNull());
        if(!pixelsHasGroup.Any())
            return;
        var firstOuterGroupSelected = new List<int>();
        foreach (var pixel in pixelsHasGroup)
        {
            var firstGroup = pixel.GetFirstGroup();
            if(firstOuterGroupSelected.Contains(firstGroup.id))
                continue;
            firstOuterGroupSelected.Add(firstGroup.id);
            var pixelsInGroup = firstGroup.GetPixelsInChildren();
            pixelsInGroup = pixelsInGroup.Where(x => !x.selecting && x.group.IsNotNull()).ToArray();
            foreach (var pixelInGroup in pixelsInGroup)
            {
                pixelInGroup.Select();
            }
            pixelsInGroup = null;
        }
        firstOuterGroupSelected.Clear();
        // active the pivot of group
        // var manySelectedGroups = GetManyGroups(pixelsHasGroup);
        // foreach (var group in manySelectedGroups)
        // {
        //     var pixelsInGroup = group.pixels;
        //     if(!pixelsInGroup.Any())
        //         continue;
        //     var pixel = pixelsInGroup.First();
        //     if (pixel.IsNotNull())
        //     {
        //         group.SetEnabledPivot(pixel.selecting);
        //         pixel = null;
        //     }

        // }
        // manySelectedGroups = null;
        pixelsHasGroup = null;
    }

    public void GetParentalGroups(ref List<Group> parentalGroups){
        var outsideGroup = parentalGroup;
        if(outsideGroup.IsNotNull())
        {
            parentalGroups.Add(outsideGroup);
            outsideGroup.GetParentalGroups(ref parentalGroups); 
        }
    }

    public void SetEnabledPivot(bool enabled)
    {
        pivot.gameObject.SetActive(enabled);
    }

    public void TogglePivot()
    {
        var pivotGo = pivot.gameObject;
        var activeSelf = pivotGo.activeSelf;
        pivot.gameObject.SetActive(!activeSelf);
    }

    public void Remove()
    {
        // var pixels = GetPixelsInChildren();
        // foreach(var pixel in pixels)
        // {
        //     var scriptHost = pixel.GetComponent<ScriptableHost>();
        //     if(scriptHost.IsNull())
        //         continue;
        //     scriptHost.RemoveAllScript();
        // }
        // foreach(var group in groupChildren)
        // {
        //     var scriptHost = group.GetComponent<ScriptableHost>();
        //     if(scriptHost.IsNull())
        //         continue;
        //     scriptHost.RemoveAllScript();
        // }
        var scriptHostOfGroup = GetComponent<ScriptableHost>();
        if(scriptHostOfGroup.IsNotNull())
        {
            if(scriptHostOfGroup.IsNotNull())
                scriptHostOfGroup.RemoveAllScript();
        }
        // var cachedParentalGroup = parentalGroup;
        // if(cachedParentalGroup.IsNotNull())
        // {
        //     UnassignParentalGroup();
        //     var groupChildrenOfParental = cachedParentalGroup.groupChildren;
        //     if(groupChildrenOfParental.Count(x => x.IsNotNull()) == 1)
        //     {
        //         DestroyImmediate(gameObject);
        //         DestroyImmediate(cachedParentalGroup.gameObject);
        //         return;
        //     }
        // }
        // _pixels = null;
        // _groupChildren = null;
        DestroyImmediate(gameObject);
    }

    public IEnumerable<Pixel> GetPixelsInChildren()
    {
        var list = new List<Pixel>();
        list.AddRange(pixels);
        GetEntirePixel(groupChildren, ref list);
        return list;
    }

    void GetEntirePixel(IEnumerable<Group> groups, ref List<Pixel> pixels)
    {
        if(!groups.Any())
            return;
        foreach(var group in groups)
        {
            pixels.AddRange(group.pixels);
            GetEntirePixel(group.groupChildren, ref pixels);
        }
    }

    public GameObject Prefabricate(GameObject patternObject, Transform prefabContainer)
    {
        var groupPrefab = Resources.Load<GameObject>(Constants.GROUP_PREFAB);
        // var prefabGo = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity, prefabContainer);
        var prefabGo = Instantiate(groupPrefab, patternObject.transform.position, Quaternion.identity);
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
        group.SetEnabledPivot(false);
        var prefabGoTransform = prefabGo.transform;
        prefabGoTransform.position = Vector3.zero;
        prefabGoTransform.SetParent(prefabContainer);
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
            var pixelsInPatternGroup = patternGroup.pixels;
            foreach (var pixel in pixelsInPatternGroup)
            {
                // PixelManager.instance.AddPixel(pixel);
                var pixelTransform = pixel.transform;
                var instancePixel = Instantiate(pixel, pixelTransform.localPosition, pixelTransform.rotation);
                instancePixel.name = pixel.name;
                group.AddPixel(instancePixel);
                pixelManager.AddPixel(instancePixel);
            }
            var groups = patternGroup.groupChildren;
            group.CloneGroup(groups, true);
            // Compute pivot of group
            var pixelsInGroup = group.GetPixelsInChildren();
            var selectedPoints = pixelsInGroup.Select(x => x.transform.position).ToArray();
            var centerPoint = TransformUtility.ComputeCenterPoint(selectedPoints);
            // var groupPosition = centerPoint.ToVector2().Snap2();
            var pivot = group.GetComponentInChildren<GroupPivot>();
            pivot.transform.position = new Vector3(centerPoint.x, centerPoint.y, pivot.transform.position.z);
        }
        group.SetEnabledPivot(false);
        return prefabGo;
    }

    public void CloneGroup(IEnumerable<Group> cloneGroupChildren, bool addToListOfPixel = false)
    {
        if(!cloneGroupChildren.Any())
            return;
        var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
        foreach(var childGroup in cloneGroupChildren)
        {
            var childGroupPosition = childGroup.transform.position;
            var prefabGo = Instantiate(groupPrefab, childGroupPosition, Quaternion.identity, transform);
            var pixels = childGroup.pixels.Select(x => {
                var xTransform = x.transform;
                var instanceX = Instantiate(x, xTransform.position, xTransform.rotation);
                instanceX.name = x.name;
                return instanceX;
            }).ToList();
            pixels.ForEach(pixel => 
            {
                prefabGo.AddPixel(pixel);
                if(addToListOfPixel)
                    pixelManager.AddPixel(pixel);
            });
            var groupChildren = childGroup.groupChildren;
            if(groupChildren.Any())
            {
                prefabGo.CloneGroup(groupChildren, addToListOfPixel);
            }
            prefabGo.SetEnabledPivot(false);
            AddChildGroup(prefabGo);
            pixels.Clear();
        }
    }
}