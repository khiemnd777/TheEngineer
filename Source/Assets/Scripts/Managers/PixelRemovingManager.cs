using UnityEngine;

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
        }
    }

    public void Remove()
    {
        var pixels = FindObjectsOfType<Pixel>();
        foreach (var pixel in pixels)
        {
            if (!pixel.selecting)
                continue;
            if (Group.HasGroup(pixel))
            {
                Group.UngroupSingle(pixel);
            }
            DestroyImmediate(pixel.gameObject);
        }
        EventObserver.instance.happeningEvent = Events.RemovePixel;
        hierarchyManager.UpdatePixelPart();
    }
}