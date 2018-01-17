using UnityEngine;
using UnityEngine.EventSystems;

public class GroupPivot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Pivot pivot;

    Vector2 _anchorMovePoint;

    void Update()
    {
        // transform.parent.RotateAround(transform.position, Vector3.forward, Time.deltaTime * 10f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var realPosition = transform.localPosition;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _anchorMovePoint = realPosition - mousePosition;

    }

    public void OnDrag(PointerEventData data)
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var mousePosition = Input.mousePosition;
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var targetPosition = worldMousePosition.ToVector2();
            var realPosition = targetPosition + _anchorMovePoint;
            realPosition = realPosition.Round2();
            transform.position = realPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}