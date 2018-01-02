using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjectManager : MonoBehaviour
{
    public RectTransform selectionPrefab;

    bool multipleChoice;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
        {
            multipleChoice = true;
        }
        else
        {
            multipleChoice = false;
        }
		
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                var pixel = hit.transform.GetComponent<Pixel>();
                if (!pixel.IsNull())
                {
                    var pixels = FindObjectsOfType<Pixel>();
                    foreach (var anotherPixel in pixels)
                    {
                        if (anotherPixel == pixel)
                        {
                            continue;
                        }
                        if (multipleChoice)
                            continue;
                        anotherPixel.Deselect();
                    }
                    if (multipleChoice)
                        if (pixel.selecting)
                            pixel.Deselect();
                        else
                            pixel.Select();
                    else
                        pixel.Select();
                }
            }
            else
            {
                // deselect all pixels
                var pixels = FindObjectsOfType<Pixel>();
                foreach (var anotherPixel in pixels)
                {
                    anotherPixel.Deselect();
                }
            }
        }
    }
}