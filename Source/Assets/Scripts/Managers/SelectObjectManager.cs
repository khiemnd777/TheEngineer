using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void OnMultipleSelecting();

public class SelectObjectManager : MonoBehaviour
{
    #region Singleton
    static SelectObjectManager _instance;

    public static SelectObjectManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SelectObjectManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active SelectObjectManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public RectTransform selectRect;

    public OnMultipleSelecting onMultipleSelecting;

    bool multipleChoice;
    bool _dragToSelect = true;
    Vector2 _anchorSelectRectPoint;

    void Update()
    {
        // OutFocusAll();
        MultipleChoice();
        // select pixel objects
        SelectPixels();
        // draw select rect
        DrawSelectRect();
    }

    void MultipleChoice()
    {
        multipleChoice =
            Input.GetKey(KeyCode.LeftCommand)
            || Input.GetKey(KeyCode.RightCommand)
            || Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.RightControl);
    }

    void OutFocusAll()
    {
        if (Input.GetMouseButtonUp(1))
        {
            EventObserver.instance.happeningEvent = Events.None;
            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                pixel.Deselect();
            }
        }
    }

    void DrawSelectRect()
    {
        if (!_dragToSelect)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            _anchorSelectRectPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButton(0))
        {
            if (selectRect.sizeDelta.x > 12.25f || selectRect.sizeDelta.y > 12.25f)
            {
                EventObserver.instance.happeningEvent = Events.DragToMultipleSelect;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }

            // selectRect.anchoredPosition = _anchorSelectRectPoint;

            var position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var diff = position - _anchorSelectRectPoint;
            if (diff == Vector2.zero)
                return;
            var startPoint = _anchorSelectRectPoint;

            if (diff.x < 0)
            {
                startPoint.x = position.x;
                diff.x = -diff.x;
            }
            if (diff.y < 0)
            {
                startPoint.y = position.y;
                diff.y = -diff.y;
            }
            selectRect.anchoredPosition = startPoint;
            selectRect.sizeDelta = diff;

            if (onMultipleSelecting.IsNotNull())
            {
                onMultipleSelecting.Invoke();
            }

            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(pixel.transform.position);
                if (RectTransformUtility.RectangleContainsScreenPoint(selectRect, screenPoint))
                {
                    if (!multipleChoice)
                    {
                        if (!pixel.selecting || !pixel.tempSelecting)
                            pixel.SelectTemp();
                    }
                    else
                    {
                        if (!pixel.selecting)
                            pixel.SelectTemp();
                        else
                            pixel.DeselectTemp();
                    }

                }
                else
                {
                    if (!multipleChoice)
                    {
                        if (pixel.selecting || pixel.tempSelecting)
                            pixel.DeselectTemp();
                    }
                    else
                    {
                        if (pixel.tempSelecting)
                            pixel.DeselectTemp();
                        if (pixel.selecting)
                        {
                            if (!pixel.tempSelecting)
                                pixel.SelectTemp();
                            else
                                pixel.DeselectTemp();
                        }
                    }
                }
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            // if(EventObserver.instance.happeningEvent == Events.OutFocusMultipleSelect){
            //     EventObserver.instance.happeningEvent = Events.None;
            // }
            if (EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
            {
                Group.SelectPixelsInGroupFollowSelectedPixel();
                return;
            }
            // reset SelectRect position
            _anchorSelectRectPoint = Vector2.zero;
            selectRect.anchoredPosition = Vector2.zero;
            selectRect.sizeDelta = Vector2.zero;
            // find any pixel has state is tempSelecting then selecting it.
            var selectNumber = 0;
            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                if (pixel.tempSelecting)
                {
                    ++selectNumber;
                    pixel.DeselectTemp();
                    pixel.Select();
                }
                else
                {
                    pixel.Deselect();
                }
            }
            // select a group follows selected pixel
            Group.SelectPixelsInGroupFollowSelectedPixel();
            // if happeningEvent was occuring DragToMultipleSelect, then assign to None.
            if (EventObserver.instance.happeningEvent == Events.DragToMultipleSelect)
            {
                if (selectNumber == 0)
                {
                    EventObserver.instance.happeningEvent = Events.None;
                }
            }
        }
    }

    void SelectPixels()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _dragToSelect = true;
            // get hit of raycast when mouse is on object
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                // just get pixel object
                var pixel = hit.transform.GetComponent<Pixel>();
                if (pixel.IsNotNull())
                {
                    if (EventObserver.instance.happeningEvent == Events.DragToMultipleSelect
                        || EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
                    {
                        return;
                    }
                    var pixels = FindObjectsOfType<Pixel>();
                    foreach (var anotherPixel in pixels)
                    {
                        if (anotherPixel == pixel || multipleChoice)
                            continue;
                        // deselect another pixel object if a pixel selected
                        // if multipleChoice actived, then, ignore below
                        anotherPixel.DeselectTemp();
                    }
                    // pixel selected
                    // if multipleChoice actived, then selects any pixels without selected
                    if (multipleChoice)
                    {
                        if (pixel.tempSelecting)
                        {
                            pixel.DeselectTemp();
                        }
                        else
                        {
                            pixel.SelectTemp();
                        }
                    }
                    else
                    {
                        pixel.SelectTemp();
                    }
                    EventObserver.instance.happeningEvent = Events.SelectPixel;
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
                return;
            if (EventObserver.instance.happeningEvent == Events.OutFocusMultipleSelect
                || EventObserver.instance.happeningEvent == Events.OutFocusSelect)
            {
                EventObserver.instance.happeningEvent = Events.None;
                return;
            }
            if (EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
            {
                EventObserver.instance.happeningEvent = Events.OutFocusMultipleSelect;
            }
            Pixel hittingPixel = null;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                // just get pixel object
                hittingPixel = hit.transform.GetComponent<Pixel>();
                if (hittingPixel.IsNotNull())
                {
                    _dragToSelect = false;
                }
            }
            // deselect all pixels if mouse is on blank space
            if (multipleChoice)
            {
                var pixels = FindObjectsOfType<Pixel>();
                var selectNumber = 0;
                foreach (var anotherPixel in pixels)
                {
                    if (anotherPixel.selecting)
                    {
                        ++selectNumber;
                        anotherPixel.SelectTemp();
                    }
                }
            }
            else
            {
                var pixels = FindObjectsOfType<Pixel>();
                if (EventObserver.instance.happeningEvent == Events.DragPixelStart || EventObserver.instance.happeningEvent == Events.DragMultiplePixelsStart)
                {
                    if (hittingPixel.IsNotNull())
                        return;
                }
                if (EventObserver.instance.happeningEvent == Events.DragToMultipleSelect)
                {
                    EventObserver.instance.happeningEvent = Events.OutFocusMultipleSelect;
                }
                if (EventObserver.instance.happeningEvent == Events.SelectPixel)
                {
                    EventObserver.instance.happeningEvent = Events.OutFocusSelect;
                }
                foreach (var anotherPixel in pixels)
                {
                    anotherPixel.Deselect();
                }
            }
        }
    }
}