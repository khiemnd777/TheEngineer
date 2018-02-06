using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PixelRemovingManager : MonoBehaviour
{
    #region Singleton
    static PixelRemovingManager _instance;

    public static PixelRemovingManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<PixelRemovingManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active PixelRemovingManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    HierarchyManager hierarchyManager;

    void Start()
    {
        hierarchyManager = HierarchyManager.instance;
    }

    void Update()
    {
        if (
            // macOS
            ((Input.GetKey(KeyCode.LeftCommand)
                || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyUp(KeyCode.Backspace))
            // windowsOS
            || Input.GetKeyUp(KeyCode.Delete)
            )
        {
            Remove();
            hierarchyManager.UpdatePixelPart();
        }
    }

    public void Remove()
    {
        // var pixels = FindObjectsOfType<Pixel>();
        var selectPixels = PixelManager.instance.GetPixels(x => x.selecting).ToArray();
        var potentialDeletingGroups = Group.GetManyGroups(selectPixels);
        Debug.Log(potentialDeletingGroups.Count);
        foreach (var pixel in selectPixels)
        {
            Remove(pixel);
        }
        var potentialDeletingGroupsIt = potentialDeletingGroups.Where(x=>x.IsNotNull()).ToList();
        Debug.Log(potentialDeletingGroupsIt.Count);
        potentialDeletingGroupsIt.ForEach(group => group.Remove());
        potentialDeletingGroupsIt.Clear();
        selectPixels = null;
        EventObserver.instance.happeningEvent = Events.RemovePixel;
    }

    public void Remove(Pixel pixel)
    {
        pixel.Remove();
    }
}