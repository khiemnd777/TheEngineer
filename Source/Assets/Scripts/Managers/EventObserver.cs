using UnityEngine;

public enum Events
{
    None, DragPixelStart, DragPixel, CreatePixel, SelectPixel, DragToMultipleSelect, OutFocusMultipleSelect, OutFocusSelect
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