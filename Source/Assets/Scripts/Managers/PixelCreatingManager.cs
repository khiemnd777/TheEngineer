using UnityEngine;
using UnityEngine.EventSystems;

public class PixelCreatingManager : MonoBehaviour
{
    void Start()
    {
        MouseEventDetector.instance.onSingleMouseUp += (position) =>
        {
            CreatePixel(position);
        };
    }

    void CreatePixel(Vector2 position)
    {
        if (EventObserver.instance.happeningEvent == Events.SelectPixel
            || EventObserver.instance.happeningEvent == Events.ShowContextMenu
            || EventObserver.instance.happeningEvent == Events.HideContextMenu
            || EventObserver.instance.happeningEvent == Events.DragPixel
            || EventObserver.instance.happeningEvent == Events.DragToMultipleSelect
            || EventObserver.instance.happeningEvent == Events.OutFocusMultipleSelect
            || EventObserver.instance.happeningEvent == Events.OutFocusSelect)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        EventObserver.instance.happeningEvent = Events.CreatePixel;

        var mousePosition = Camera.main.ScreenToWorldPoint(position);
        var pixelPosition = mousePosition.ToVector2().Round2();
        var pixelPrefab = Resources.Load<Pixel>(Constants.PIXEL_PREFAB);
        // create an instance of pixel
        Instantiate<Pixel>(pixelPrefab, pixelPosition, Quaternion.identity);
        pixelPrefab = null;
    }
}