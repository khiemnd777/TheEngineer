using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjectManager : MonoBehaviour
{
    public RectTransform selectRect;

    bool multipleChoice;
    bool _dragToSelect = true;
    Vector2 _anchorSelectRectPoint;

    void Update()
    {
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

    void DrawSelectRect()
    {
        if (!_dragToSelect)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            _anchorSelectRectPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            selectRect.anchoredPosition = _anchorSelectRectPoint;
        }
        if (Input.GetMouseButton(0))
        {
            var position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var diff = position - _anchorSelectRectPoint;
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
            _anchorSelectRectPoint = Vector2.zero;
            selectRect.anchoredPosition = Vector2.zero;
            selectRect.sizeDelta = Vector2.zero;

            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                if (pixel.tempSelecting)
                {
                    pixel.DeselectTemp();
                    pixel.Select();
                }
                else
                    pixel.Deselect();
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
                        pixel.SelectTemp();
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                // just get pixel object
                var pixel = hit.transform.GetComponent<Pixel>();
                if (pixel.IsNotNull())
                {
                    _dragToSelect = false;
                }
            }
            // deselect all pixels if mouse is on blank space
            if (multipleChoice)
            {
                var pixels = FindObjectsOfType<Pixel>();
                foreach (var anotherPixel in pixels)
                {
                    if (anotherPixel.selecting)
                        anotherPixel.SelectTemp();
                }
            }
            else
            {
                var pixels = FindObjectsOfType<Pixel>();
                foreach (var anotherPixel in pixels)
                {
                    anotherPixel.Deselect();
                }
            }
        }
    }
}