using UnityEngine;

public class DragUtility
{
    bool mouseDown = false;
    Vector3 startMousePos;
    Vector3 startPos;
    bool restrictX;
    bool restrictY;
    float fakeX;
    float fakeY;
    float myWidth;
    float myHeight;

    RectTransform _parentRectTransform;
    RectTransform _selfRectTransform;
    Transform _selfTransform;

    public DragUtility(Transform selfTransform, RectTransform selfRectTransform, RectTransform parentRectTransform = null)
    {
        _selfTransform = selfTransform;
        _selfRectTransform = selfRectTransform;
        _parentRectTransform = (parentRectTransform == null || parentRectTransform is Object && parentRectTransform.Equals(null))
            ? selfRectTransform.parent.GetComponent<RectTransform>()
            : _parentRectTransform = parentRectTransform;

        myWidth = (_selfRectTransform.rect.width + 5) / 2;
        myHeight = (_selfRectTransform.rect.height + 5) / 2;
    }

    public void DragStart(Vector3 startMousePosition)
    {
        mouseDown = true;
        startPos = _selfTransform.position;
        startMousePos = startMousePosition;
    }

    public void Drag(Vector3 mousePosition)
    {
        if(!mouseDown)
            return;
        var currentPos = mousePosition;
        var diff = currentPos - startMousePos;
        var pos = startPos + diff;
        var localPosition = _selfTransform.localPosition;
        var parentWidth = _parentRectTransform.rect.width / 2;
        var parentHeight = _parentRectTransform.rect.height / 2;

        _selfTransform.position = pos;

        restrictX = _selfTransform.localPosition.x < 0 - (parentWidth - myWidth)
            || _selfTransform.localPosition.x > (parentWidth - myWidth);
        restrictY = _selfTransform.localPosition.y < 0 - (parentHeight - myHeight)
            || _selfTransform.localPosition.y > (parentHeight - myHeight);

        if (restrictX)
        {
            fakeX = _selfTransform.localPosition.x < 0
                ? 0 - parentWidth + myWidth
                : parentWidth - myWidth;

            var xpos = new Vector3(fakeX, _selfTransform.localPosition.y, 0.0f);
            _selfTransform.localPosition = xpos;
        }

        if (restrictY)
        {
            fakeY = _selfTransform.localPosition.y < 0
                ? 0 - parentHeight + myHeight
                : parentHeight - myHeight;

            var ypos = new Vector3(_selfTransform.localPosition.x, fakeY, 0.0f);
            _selfTransform.localPosition = ypos;
        }
    }

    public void DragEnd()
    {
        mouseDown = false;
    }
}