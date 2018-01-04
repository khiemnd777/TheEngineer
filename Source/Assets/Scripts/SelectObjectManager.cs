using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjectManager : MonoBehaviour
{
    public RectTransform selectRect;

    bool multipleChoice;
    bool _mouseMoveToSelect;
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
        if (Input.GetMouseButtonDown(0))
        {
            _anchorSelectRectPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            selectRect.anchoredPosition = _anchorSelectRectPoint;
            StartCoroutine("SelectRectSelect");
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
            _mouseMoveToSelect = diff.sqrMagnitude > 1;

            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(pixel.transform.position);
                if (RectTransformUtility.RectangleContainsScreenPoint(selectRect, screenPoint))
                {
                    if(!multipleChoice){
                        if(!pixel.selecting)
                            pixel.Select();
                    }else{
                        if(!pixel.selecting)
                            pixel.Select();
                    }
                    // if (!pixel.selecting)
                    //     if(!multipleChoice)
                    //         pixel.Select();
                    //     else
                    //         pixel.Deselect();
                    // else
                    //     if(multipleChoice)
                    //         pixel.Deselect();
                    //     else
                    //         pixel.Select();
                        
                }
                else
                {
                    if(multipleChoice){
                        
                    }else{
                        if(pixel.selecting)
                            pixel.Deselect();
                    }
                }
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseMoveToSelect = false;
            _anchorSelectRectPoint = Vector2.zero;
            selectRect.anchoredPosition = Vector2.zero;
            selectRect.sizeDelta = Vector2.zero;
            StopCoroutine("SelectRectSelect");
        }
    }

    IEnumerator SelectRectSelect()
    {
        while (true)
        {
            yield return null;
            if(!_mouseMoveToSelect)
                continue;
            
        }
    }

    void SelectPixels()
    {
        if (Input.GetMouseButtonUp(0))
        {
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
                        anotherPixel.Deselect();
                    }
                    // pixel selected
                    // if multipleChoice actived, then selects any pixels without selected
                    if (multipleChoice)
                        if (pixel.selecting)
                            pixel.Deselect();
                        else
                            pixel.Select();
                    else
                        pixel.Select();
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            // deselect all pixels if mouse is on blank space
            if (multipleChoice)
                return;
            var pixels = FindObjectsOfType<Pixel>();
            foreach (var anotherPixel in pixels)
            {
                anotherPixel.Deselect();
            }
        }
    }
}