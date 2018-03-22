using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DragUI : MonoBehaviour
    , IPointerDownHandler
    , IPointerUpHandler
{
    public RectTransform parentRectTransform;
    [System.NonSerialized]
    public RectTransform selfRectTransform;

    Canvas _canvas;

    bool mouseDown = false;
    Vector3 startMousePos;
    Vector3 startPos;
    bool restrictX;
    bool restrictY;
    float fakeX;
    float fakeY;
    float myWidth;
    float myHeight;

    public virtual void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        selfRectTransform = GetComponent<RectTransform>();
        if (parentRectTransform == null || parentRectTransform is Object && parentRectTransform.Equals(null))
        {
            parentRectTransform = selfRectTransform.parent.GetComponent<RectTransform>();
        }

        myWidth = (selfRectTransform.rect.width + 5) / 2;
        myHeight = (selfRectTransform.rect.height + 5) / 2;
    }

    public virtual void Update()
    {
        if (mouseDown)
        {
            Drag();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DragStart();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DragEnd();
    }

    public virtual void DragStart()
    {
        mouseDown = true;
        startPos = transform.position;
        startMousePos = Input.mousePosition;
        transform.SetAsLastSibling();
    }

    public virtual void Drag()
    {
        var currentPos = Input.mousePosition;
        var diff = currentPos - startMousePos;
        var pos = startPos + diff;
        var localPosition = transform.localPosition;
        var parentWidth = parentRectTransform.rect.width / 2;
        var parentHeight = parentRectTransform.rect.height / 2;

        transform.position = pos;

        restrictX = transform.localPosition.x < 0 - (parentWidth - myWidth)
            || transform.localPosition.x > (parentWidth - myWidth);
        restrictY = transform.localPosition.y < 0 - (parentHeight - myHeight)
            || transform.localPosition.y > (parentHeight - myHeight);

        if (restrictX)
        {
            fakeX = transform.localPosition.x < 0
                ? 0 - parentWidth + myWidth
                : parentWidth - myWidth;

            var xpos = new Vector3(fakeX, transform.localPosition.y, 0.0f);
            transform.localPosition = xpos;
        }

        if (restrictY)
        {
            fakeY = transform.localPosition.y < 0
                ? 0 - parentHeight + myHeight
                : parentHeight - myHeight;

            var ypos = new Vector3(transform.localPosition.x, fakeY, 0.0f);
            transform.localPosition = ypos;
        }
    }

    public virtual void DragEnd()
    {
        mouseDown = false;
    }
}