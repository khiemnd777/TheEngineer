using UnityEngine;

public enum Events
{
    None
    , DragPivotStart
    , DragPivot
    , DragPixelStart
    , DragMultiplePixelsStart
    , DragPixel
    , DragMultiplePixels
    , DropPivot
    , CreatePixel
    , SelectPixel
    , DragToMultipleSelect
    , OutFocusMultipleSelect
    , OutFocusSelect
    , ShowContextMenu
    , HideContextMenu
    , RemovePixel
    , DragHierarchyItem
    , CloseScriptPanel
}

public class EventObserver : MonoBehaviour
{
    #region Singleton
    static EventObserver _instance;

    public static EventObserver instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<EventObserver>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active EventObserver script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public Events happeningEvent;
}