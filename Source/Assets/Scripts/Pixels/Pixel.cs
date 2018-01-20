using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public delegate void OnDragEvent(Vector2 position);

public class Pixel : MonoBehaviour
{
    public int id;
    public TextMesh text;
    public bool selecting;
    public bool tempSelecting;
    public bool draggedHold;
    public PixelPivot pivot;
    public Transform selection;
    public Transform hoverable;
    public Transform colliderGroup;
    public Transform[] anchors;

    // events
    public OnDragEvent onDragStart;
    public OnDragEvent onDrag;
    public OnDragEvent onDrop;

    public ExpandoObject pythonPixel;

    Vector2 _anchorMovePoint;

    List<Scriptable> scriptableList;

    static int _currentID;

    public static int GetUniqueID()
    {
        return ++_currentID;
    }

    void Start()
    {
        // id
        id = GetUniqueID();
        name = "Pixel " + id;
        text.text = name;
        // instance python's scriptable object
        pythonPixel = new ExpandoObject();
        ExpandoObjectUtility.SetVariable(pythonPixel, "id", GetInstanceID());
        ExpandoObjectUtility.SetVariable(pythonPixel, "position", new Position
        {
            x = transform.position.x,
            y = transform.position.y
        });
        StartCoroutine(SetPythonPixelPosition());
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    void DragStart()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            // if hitting the pivot of group
            var hittingPivot = hit.transform.GetComponent<GroupPivot>();
            if (hittingPivot.IsNotNull())
            {
                return;
            }
        }
        var pixels = FindObjectsOfType<Pixel>();
        // if this pixel is non-selecting, then deselecting all another one
        if (!selecting && !SelectObjectManager.instance.multipleChoice)
        {
            var pixelsWithoutThis = pixels.Where(x => x.GetID() != this.GetID() && x.selecting);
            if (pixelsWithoutThis.Any())
            {
                foreach (var p in pixelsWithoutThis.ToArray())
                {
                    p.Deselect();
                }
            }
        }
        var selectedPixels = pixels.Where(x => x.selecting).ToList();
        var realPosition = transform.localPosition;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // if multipleChoice was actived, then denying dragging start of group
        if (selectedPixels.Count > 1 && !SelectObjectManager.instance.multipleChoice)
        {
            Group.Create();
            var group = Group.GetFirstGroup(this);
            if (group.IsNotNull())
            {
                realPosition = group.transform.localPosition;
            }
            EventObserver.instance.happeningEvent = Events.DragMultiplePixelsStart;
        }
        else
        {
            // if pixel has been in any group, then getting world position of it for computing accurately.
            if (Group.HasGroup(this))
            {
                var group = Group.GetFirstGroup(this);
                if (group.IsNotNull())
                {
                    realPosition = group.transform.localPosition;
                }
            }
            EventObserver.instance.happeningEvent = Events.DragPixelStart;
        }
        _anchorMovePoint = realPosition - mousePosition;
        // if multipleChoice was actived, then denying of dragging
        draggedHold = true && !SelectObjectManager.instance.multipleChoice;
        if (onDragStart.IsNotNull())
        {
            onDragStart.Invoke(transform.position);
        }
        pixels = null;
    }

    void Drag()
    {
        if (draggedHold)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                if (EventObserver.instance.happeningEvent == Events.DragPixelStart)
                    EventObserver.instance.happeningEvent = Events.DragPixel;
                if (EventObserver.instance.happeningEvent == Events.DragMultiplePixelsStart)
                    EventObserver.instance.happeningEvent = Events.DragMultiplePixels;

                // move if mouse has any movement
                var mousePosition = Input.mousePosition;
                var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                var targetPosition = worldMousePosition.ToVector2();
                var realPosition = targetPosition + _anchorMovePoint;
                realPosition = realPosition.Snap2();
                if (Group.HasGroup(this))
                {
                    // if pixel has been in a group, then move that group
                    var group = Group.GetFirstGroup(this);
                    if (group.IsNotNull())
                    {
                        group.transform.position = realPosition;
                    }
                }
                else
                {
                    // if pixel was in normal state
                    transform.position = realPosition;
                }

                if (onDrag.IsNotNull())
                {
                    onDrag.Invoke(realPosition);
                }
            }
        }
    }

    void Drop()
    {
        draggedHold = false;
        if (EventObserver.instance.happeningEvent == Events.DragMultiplePixelsStart
            || EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
        {
            Group.UngroupOneByOne();
        }
        else
        {
            var closestPixel = GetClosestPixel();
            if (closestPixel.IsNotNull())
            {
                var closestAnchor = GetClosestAnchor(closestPixel);
                if (closestAnchor.IsNotNull())
                {
                    transform.position = closestAnchor.transform.position;
                }
            }
        }
        if (onDrop.IsNotNull())
        {
            onDrop.Invoke(transform.position);
        }
    }

    void OnMouseDown()
    {
        DragStart();
    }

    void OnMouseUp()
    {
        Drop();
    }

    void OnMouseOver()
    {
        if (selecting)
            return;
        VisibleHoverable(true);
    }

    void OnMouseExit()
    {
        if (selecting)
            return;
        VisibleHoverable(false);
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public Pixel GetClosestPixel()
    {
        var bestPotential = TransformUtility.FindClosestObjectsOfType<Pixel>(transform.position, Constants.CLOSEST_PIXEL_DISTANCE, x => x != this);
        return bestPotential;
    }

    public Transform GetClosestAnchor(Pixel pixel)
    {
        var bestPotential = TransformUtility.FindClosestObjectsBySpecific<Transform>(transform.position, Constants.CLOSEST_ANCHOR_DISTANCE, pixel.anchors);
        return bestPotential;
    }

    public void Select()
    {
        selecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
        pivot.gameObject.SetActive(!Group.HasGroup(this));
    }

    public void Deselect()
    {
        selecting = false;
        VisibleSelection(false);
        if(EventObserver.instance.happeningEvent != Events.DragPivotStart)
            pivot.gameObject.SetActive(false);
    }

    public void SelectTemp()
    {
        tempSelecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
    }

    public void DeselectTemp()
    {
        tempSelecting = false;
        VisibleSelection(false);
    }

    void VisibleHoverable(bool visible)
    {
        if (!selecting && !tempSelecting)
            text.gameObject.SetActive(visible);
        hoverable.gameObject.SetActive(visible);
    }

    void VisibleSelection(bool visible)
    {
        text.gameObject.SetActive(visible);
        selection.gameObject.SetActive(visible);
    }

    public bool IsInGroup()
    {
        var group = GetComponentInParent<Group>();
        return group.IsNotNull();
    }

    IEnumerator SetPythonPixelPosition()
    {
        while (!gameObject.IsNull())
        {
            yield return null;
            if (!gameObject.activeSelf)
                continue;
            var position = (Position)ExpandoObjectUtility.GetVariable(pythonPixel, "position");
            if (position.x == transform.position.x && position.y == transform.position.y)
                continue;
            ExpandoObjectUtility.SetVariable(pythonPixel, "position", new Position
            {
                x = transform.position.x,
                y = transform.position.y
            });
        }
    }
}