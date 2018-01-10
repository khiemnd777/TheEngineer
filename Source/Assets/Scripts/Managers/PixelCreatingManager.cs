using UnityEngine;
using UnityEngine.EventSystems;

public class PixelCreatingManager : MonoBehaviour
{
    void Start()
    {
        MouseEventDetector.instance.onSingleMouseUp += () =>
        {
            CreatePixel();
        };
    }

    void CreatePixel()
    {
        if (EventObserver.instance.happeningEvent == Events.SelectPixel
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

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = mousePosition.ToVector2().Round2();

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider.IsNotNull())
            return;

        var pixelPrefab = Resources.Load<Pixel>(Constants.PIXEL_PREFAB);
        // create an instance of pixel
        var instancePixel = Instantiate<Pixel>(pixelPrefab, pixelPosition, Quaternion.identity);
        // find out a closest pixel excepts instance pixel
        var closestPixel = TransformUtility.FindClosestObjectsOfType<Pixel>(instancePixel.transform.position, Constants.CLOSEST_PIXEL_DISTANCE, x => x != instancePixel);
        if (closestPixel.IsNotNull())
        {
            // find out a closest anchor of closest pixel
            var closestAnchor = TransformUtility.FindClosestObjectsBySpecific<Transform>(instancePixel.transform.position, Constants.CLOSEST_ANCHOR_DISTANCE, closestPixel.anchors);
            if (closestAnchor.IsNotNull())
            {
                // assign position of closest anchor to instance of pixel
                instancePixel.transform.position = closestAnchor.transform.position;
                closestAnchor = null;
            }
            closestPixel = null;
        }
        instancePixel = null;
        pixelPrefab = null;
    }
}