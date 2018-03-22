using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintEntityDrag : MonoBehaviour
    , IPointerDownHandler
    , IPointerUpHandler
{
    DragUtility _dragUtility;

    void Start()
    {
        _dragUtility = new DragUtility(transform.parent
            , transform.parent.GetComponent<RectTransform>()
            , transform.parent.parent.GetComponent<RectTransform>());
    }

    void Update()
    {
        _dragUtility.Drag(Input.mousePosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragUtility.DragStart(Input.mousePosition);
        transform.parent.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _dragUtility.DragEnd();
    }
}