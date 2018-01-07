using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public delegate void OnDragStart(Vector2 position);
public delegate void OnDrag(Vector2 position);
public delegate void OnDrop(Vector2 position);

public class Pixel : MonoBehaviour
{
    public int id;
    public TextMesh text;
    public bool selecting;
    public bool tempSelecting;
    public bool draggedHold;
    public Transform selection;
    public Transform hoverable;
    public Transform colliderGroup;
    public Transform[] anchors;
    
    // events
    public OnDragStart onDragStart;
    public OnDrag onDrag;
    public OnDrop onDrop;

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
        _anchorMovePoint = transform.localPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggedHold = true;
        EventObserver.instance.happeningEvent = Events.DragPixelStart;
        if (onDragStart.IsNotNull())
        {
            onDragStart.Invoke(transform.position);
        }
    }

    void Drag()
    {
        if (draggedHold)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                EventObserver.instance.happeningEvent = Events.DragPixel;
                // move if mouse has any movement
                var mousePosition = Input.mousePosition;
                var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                var targetPosition = new Vector2(worldMousePosition.x, worldMousePosition.y);
                var realPosition = targetPosition + _anchorMovePoint;
                transform.position = realPosition;
                
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
        var closestPixel = GetClosestPixel();
        if (closestPixel.IsNotNull())
        {
            var closestAnchor = GetClosestAnchor(closestPixel);
            if (closestAnchor.IsNotNull())
            {
                transform.position = closestAnchor.transform.position;
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
    }

    public void Deselect()
    {
        selecting = false;
        VisibleSelection(false);
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

    public void AddScriptable(Scriptable scriptable)
    {
        if (scriptableList.IsNull())
            scriptableList = new List<Scriptable>();
        scriptable.pixel = this;
        scriptableList.Add(scriptable);
    }

    public T GetScriptable<T>() where T : Scriptable
    {
        if (scriptableList.IsNull())
            return null;
        var scriptable = scriptableList.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.GetType()));
        return (T)scriptable;
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