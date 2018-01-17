using UnityEngine;
using UnityEngine.EventSystems;

public class GroupPivot : MonoBehaviour
{
    public Pivot pivot;

    Vector2 _anchorMovePoint;

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
    }

    public void Drag()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if(EventObserver.instance.happeningEvent == Events.DragPivotStart)
                EventObserver.instance.happeningEvent = Events.DragPivot;
            var mousePosition = Input.mousePosition;
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var targetPosition = worldMousePosition.ToVector2();
            var realPosition = targetPosition.RoundHalf2();// + _anchorMovePoint;
            // realPosition = realPosition.RoundHalf2();
            transform.position = new Vector3(realPosition.x, realPosition.y, transform.position.z);
        }
    }

    void Drop()
    {
        if(EventObserver.instance.happeningEvent == Events.DragPivotStart
            || EventObserver.instance.happeningEvent == Events.DragPivot)
        {

        }
    }
}