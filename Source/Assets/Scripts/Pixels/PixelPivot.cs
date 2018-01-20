using UnityEngine;
using UnityEngine.EventSystems;

public class PixelPivot : MonoBehaviour
{
    public Pivot pivot;

    Vector2 _anchorMovePoint;
    bool _draggedHold;

    void Update()
    {
        // transform.parent.RotateAround(transform.position, Vector3.forward, Time.deltaTime * 10f);
        if (Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    void OnMouseDown()
    {
        DragStart();
    }

    void OnMouseUp()
    {
        Drop();
    }

    public void DragStart()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _anchorMovePoint = transform.position - mousePosition;
        EventObserver.instance.happeningEvent = Events.DragPivotStart;
        _draggedHold = true;
    }

    public void Drag()
    {
        if(!_draggedHold)
            return;
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if(EventObserver.instance.happeningEvent == Events.DragPivotStart)
                EventObserver.instance.happeningEvent = Events.DragPivot;
            var mousePosition = Input.mousePosition;
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var targetPosition = worldMousePosition.ToVector2();
            var realPosition = (targetPosition + _anchorMovePoint).Snap2(Constants.GROUP_PIVOT_SNAP_DELTA);
            transform.position = new Vector3(realPosition.x, realPosition.y, transform.position.z);
        }
    }

    void Drop()
    {
        _draggedHold = false;
        if(EventObserver.instance.happeningEvent == Events.DragPivotStart
            || EventObserver.instance.happeningEvent == Events.DragPivot)
        {

        }
    }
}