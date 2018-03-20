using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BlueprintEntity : MonoBehaviour
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    public BlockBehaviourEntity behaviourEntity;
    public RectTransform editor;
    [System.NonSerialized]
    public RectTransform rectTransform;

    Vector3 _anchorPoint;
    RectTransform parent;

    public virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _anchorPoint = new Vector3(eventData.position.x, eventData.position.y, 0f);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0f);
        var position = transform.position;
        var distance = position - _anchorPoint;
        var realPosition = mousePosition + distance;
        var width = rectTransform.rect.width;
        var height = rectTransform.rect.height;
        var deltaWidth = width / 2f - Mathf.Abs(distance.x);
        var deltaHeight = height / 2f - Mathf.Abs(distance.y);
        Debug.Log(distance.x);
        var checkContainingLeftConner = mousePosition;
        var checkContainingRightConner = mousePosition;
        if (distance.x < 0)
        {
            if (distance.y < 0)
            {
                checkContainingLeftConner = new Vector3(mousePosition.x + deltaWidth, mousePosition.y + deltaHeight, mousePosition.z);
                checkContainingRightConner = new Vector3(mousePosition.x - (width - deltaWidth), mousePosition.y - (height - deltaHeight), mousePosition.z);
            }
            else
            {
                checkContainingLeftConner = new Vector3(mousePosition.x + deltaWidth, mousePosition.y - deltaHeight, mousePosition.z);
                checkContainingRightConner = new Vector3(mousePosition.x - (width - deltaWidth), mousePosition.y + (height - deltaHeight), mousePosition.z);
            }
        }
        else
        {
            if (distance.y < 0)
            {
                checkContainingLeftConner = new Vector3(mousePosition.x - deltaWidth, mousePosition.y + deltaHeight, mousePosition.z);
                checkContainingRightConner = new Vector3(mousePosition.x + (width - deltaWidth), mousePosition.y - (height - deltaHeight), mousePosition.z);
            }
            else
            {
                checkContainingLeftConner = new Vector3(mousePosition.x - deltaWidth, mousePosition.y - deltaHeight, mousePosition.z);
                checkContainingRightConner = new Vector3(mousePosition.x + (width - deltaWidth), mousePosition.y + (height - deltaHeight), mousePosition.z);
            }
        }
        if (!RectTransformUtility.RectangleContainsScreenPoint(editor, checkContainingLeftConner))
        {
            _anchorPoint = mousePosition;
            return;
        }
        if (!RectTransformUtility.RectangleContainsScreenPoint(editor, checkContainingRightConner))
        {
            _anchorPoint = mousePosition;
            return;
        }
        transform.position = realPosition;
        _anchorPoint = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _anchorPoint = Vector3.zero;
    }
}