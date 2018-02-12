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
    [System.NonSerialized]
    public bool multipleChoice;

    bool _dragToSelect = true;
    Vector2 _anchorSelectRectPoint;
    List<int> _lastPixelIdsInRect;
    PixelManager pixelManager;

    void Start()
    {
        _lastPixelIdsInRect = new List<int>();
        pixelManager = PixelManager.instance;
        StartCoroutine(Visualizing());
    }

    void MultipleChoice()
    {
        multipleChoice =
            Input.GetKey(KeyCode.LeftCommand)
            || Input.GetKey(KeyCode.RightCommand)
            || Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.RightControl);
    }

    IEnumerator Visualizing()
    {
        while(true)
        {
            // OutFocusAll();
            MultipleChoice();
            // select pixel objects
            SelectPixels();
            // draw select rect
            DrawSelectRect();
            yield return null;
        }
    }

    void OutFocusAll()
    {
        if (Input.GetMouseButtonUp(1))
        {
            EventObserver.instance.happeningEvent = Events.None;
            // var pixels = FindObjectsOfType<Pixel>();
            var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting).ToArray();
            foreach (var pixel in selectedPixels)
            {
                pixel.Deselect();
            }
        }
    }

    void DrawSelectRect()
    {
        if (!_dragToSelect)
            return;
        if (EventObserver.instance.happeningEvent == Events.DragPivotStart
            || EventObserver.instance.happeningEvent == Events.DragPivot
            || EventObserver.instance.happeningEvent == Events.DragHierarchyItem)
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
            var mainCamera = Camera.main;
            var pixelsInRect = pixelManager.GetPixels(x => 
            {
                if(!x.gameObject.activeInHierarchy)
                    return false;
                var screenPoint = mainCamera.WorldToScreenPoint(x.transform.position);
                return RectTransformUtility.RectangleContainsScreenPoint(selectRect, screenPoint);
            });
            var pixelInRectIt = pixelsInRect.ToList();
            pixelInRectIt.ForEach(pixel => 
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
            });
            // deselect of pixels out of rect
            var hasLastSelectedPixels = _lastPixelIdsInRect.Any();
            var pixelInRectIds = pixelsInRect.Select(x => x.id).ToList();
            if(hasLastSelectedPixels)
            {
                var pixelIdsOutOfRect = _lastPixelIdsInRect.Where(x=>!pixelInRectIds.Contains(x)).ToList();
                var pixelOutOfRect = pixelManager.GetPixels(x => pixelIdsOutOfRect.Contains(x.id));
                if(pixelOutOfRect.Any())
                {
                    var pixelOutOfRectIt = pixelOutOfRect.ToList();
                    pixelOutOfRectIt.ForEach(pixel =>
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
                    });
                    pixelOutOfRectIt.Clear();
                }
            }
            _lastPixelIdsInRect.Clear();
            _lastPixelIdsInRect = pixelInRectIds;
        }
        if (Input.GetMouseButtonUp(0))
        {
            // if(EventObserver.instance.happeningEvent == Events.OutFocusMultipleSelect){
            //     EventObserver.instance.happeningEvent = Events.None;
            // }
            if (EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
            {
                // Group.SelectPixelsInGroupFollowSelectedPixel();
                return;
            }
            // reset SelectRect position
            _anchorSelectRectPoint = Vector2.zero;
            selectRect.anchoredPosition = Vector2.zero;
            selectRect.sizeDelta = Vector2.zero;
            // find any pixel has state is tempSelecting then selecting it.
            var selectingPixels = pixelManager.GetPixels(x => x.tempSelecting);
            var selectingPixelsIt = selectingPixels.ToList();
            var selectNumber = selectingPixels.Count();
            if(_lastPixelIdsInRect.Any())
            {
                var pixelsInRect = pixelManager.GetPixels(x => _lastPixelIdsInRect.Contains(x.id));
                if(pixelsInRect.Any())
                {
                    var pixelsInRectIt = pixelsInRect.ToList();
                    pixelsInRectIt.ForEach(pixel => 
                    {
                        if(!pixel.tempSelecting)
                            pixel.Deselect();
                    });
                    pixelsInRectIt.Clear();
                    pixelsInRect = null;
                }
            }
            selectingPixelsIt.ForEach(pixel =>
            {
                pixel.DeselectTemp();
                pixel.Select();
            });
            selectingPixelsIt.Clear();
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
                    var pixels = PixelManager.instance.GetPixels(x => x.id != pixel.id && !multipleChoice).ToList();
                    pixels.ForEach(x => x.DeselectTemp());
                    pixels.Clear();
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
                || EventObserver.instance.happeningEvent == Events.OutFocusSelect
                || EventObserver.instance.happeningEvent == Events.RemovePixel
                || EventObserver.instance.happeningEvent == Events.CloseScriptPanel)
            {
                EventObserver.instance.happeningEvent = Events.None;
                return;
            }
            if (EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
            {
                EventObserver.instance.happeningEvent = Events.OutFocusMultipleSelect;
            }
            if (EventObserver.instance.happeningEvent == Events.DragHierarchyItem)
            {
                EventObserver.instance.happeningEvent = Events.None;
            }
            if (multipleChoice)
            {
                var selectedPixels = PixelManager.instance.GetPixels(x => x.selecting);
                var selectedPixelsIt = selectedPixels.ToList();
                selectedPixelsIt.ForEach(x => 
                {
                    x.SelectTemp();
                });
                return;
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
            if (EventObserver.instance.happeningEvent == Events.DragPixelStart
                || EventObserver.instance.happeningEvent == Events.DragMultiplePixelsStart
                || EventObserver.instance.happeningEvent == Events.DragPivotStart)
            {
                if (hittingPixel.IsNotNull())
                    return;
            }
            if(EventObserver.instance.happeningEvent == Events.DragPivotStart)
                return;
            if (EventObserver.instance.happeningEvent == Events.DragToMultipleSelect)
            {
                EventObserver.instance.happeningEvent = Events.OutFocusMultipleSelect;
            }
            if (EventObserver.instance.happeningEvent == Events.SelectPixel)
            {
                EventObserver.instance.happeningEvent = Events.OutFocusSelect;
            }
            var listOfselectedPixels = PixelManager.instance.GetPixels(x => x.selecting).ToList();
            listOfselectedPixels.ForEach(x =>
            {
                // if (x.group.IsNotNull())
                // {
                //     var firstOuterGroup = Group.GetFirstGroup(x);
                //     if (firstOuterGroup.IsNotNull())
                //     {
                //         firstOuterGroup.SetEnabledPivot(false);
                //     }
                // }
                x.Deselect();
            });
        }
    }
}