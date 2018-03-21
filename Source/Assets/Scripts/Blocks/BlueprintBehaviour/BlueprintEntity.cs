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
    Vector2 _anchorPoint2;
    Canvas _canvas;

    public virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _anchorPoint = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(editor, Input.mousePosition, Camera.main, out _anchorPoint2);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var width = rectTransform.rect.width;
        var height = rectTransform.rect.height;

        var anchorPosition = rectTransform.anchoredPosition;
        var relX = Input.mousePosition.x - Mathf.Abs(editor.rect.x);
        Debug.Log(eventData.position);
        var distance2 = anchorPosition - _anchorPoint2;
        var deltaWidth2 = width / 2f - distance2.x;
        var deltaHeight2 = height / 2f - distance2.y;

        var mousePosition = Input.mousePosition;
        var position = transform.position;
        var distance = position - _anchorPoint;
        var realPosition = mousePosition + distance;
        
        var deltaWidth = width / 2f - Mathf.Abs(distance.x);
        var deltaHeight = height / 2f - Mathf.Abs(distance.y);
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(editor, Input.mousePosition, Camera.main, out _anchorPoint2);
            return;
        }
        if (!RectTransformUtility.RectangleContainsScreenPoint(editor, checkContainingRightConner))
        {
            _anchorPoint = mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(editor, Input.mousePosition, Camera.main, out _anchorPoint2);
            return;
        }
        transform.position = realPosition;
        _anchorPoint = mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(editor, Input.mousePosition, Camera.main, out _anchorPoint2);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _anchorPoint = Vector3.zero;
    }
}