using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public delegate void OnDragEvent(object sender, Vector2 position);

public class Pixel : MonoBehaviour, IPrefabricated
{
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

    public bool isPrefab { get; set; }

    PixelManager pixelManager;

    Vector2 _anchorMovePoint;

    List<Scriptable> scriptableList;

    int _id;

    public Transform parent
    {
        get
        {
            return transform.parent;
        }
    }

    Group _group;

    public Group group
    {
        get { return _group; }
    }

    public int id
    {
        get { return _id; }
    }

    void Awake()
    {
        pixelManager = PixelManager.instance;
    }

    void Start()
    {
        _id = this.GetID();
        text.text = name;
        // instance python's scriptable object
        pythonPixel = new ExpandoObject();
        ExpandoObjectUtility.SetVariable(pythonPixel, "id", GetInstanceID());
        ExpandoObjectUtility.SetVariable(pythonPixel, "position", new Position
        {
            x = transform.position.x,
            y = transform.position.y
        });
        // StartCoroutine(SetPythonPixelPosition());
        StartCoroutine(Dragging());
    }

    IEnumerator Dragging()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                Drag();
            }
            yield return null;
        }
    }

    List<Pixel> _cachedSelectedPixels;

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
        // if this pixel is non-selecting, then deselecting all another one
        if (!selecting && !SelectObjectManager.instance.multipleChoice)
        {
            var pixelsWithoutThis = pixelManager.GetPixels(x => x.id != this.id && x.selecting);
            if (pixelsWithoutThis.Any())
            {
                var pixelsWithoutThisToArray = pixelsWithoutThis.ToArray();
                foreach (var p in pixelsWithoutThisToArray)
                {
                    p.Deselect();
                }
                pixelsWithoutThisToArray = null;
            }
        }
        var selectedPixels = pixelManager.GetPixels(x => x.selecting);
        var countOfSelectedPixels = selectedPixels.Count();
        var realPosition = transform.localPosition;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // if multipleChoice was actived, then denying dragging start of group
        if (countOfSelectedPixels > 1 && !SelectObjectManager.instance.multipleChoice)
        {
            if (_cachedSelectedPixels.IsNull())
                _cachedSelectedPixels = new List<Pixel>();
            _cachedSelectedPixels.Clear();
            _cachedSelectedPixels = selectedPixels.ToList();
            // var selectedPoints = _cachedSelectedPixels.Select(x => x.transform.position).ToArray();
            // var centerPoint = TransformUtility.ComputeCenterPoint(selectedPoints);
            // var selectedPosition = centerPoint.ToVector2();
            // realPosition = selectedPosition;
            // selectedPoints = null;
            _anchorMovePoint = mousePosition;
            // Group.Create();
            // var group = GetFirstGroup();
            // if (group.IsNotNull())
            // {
            //     realPosition = group.transform.localPosition;
            // }

            realPosition = transform.position;
            EventObserver.instance.happeningEvent = Events.DragMultiplePixelsStart;
        }
        else
        {
            // if pixel has been in any group, then getting world position of it for computing accurately.
            if (group.IsNotNull())
            {
                var group = GetFirstGroup();
                if (group.IsNotNull())
                {
                    realPosition = group.transform.localPosition;
                }
            }
            _anchorMovePoint = realPosition - mousePosition;
            EventObserver.instance.happeningEvent = Events.DragPixelStart;
        }
        // _anchorMovePoint = realPosition - mousePosition;
        // if multipleChoice was actived, then denying of dragging
        draggedHold = true && !SelectObjectManager.instance.multipleChoice;
        if (onDragStart.IsNotNull())
        {
            onDragStart.Invoke(this, transform.position);
        }
        selectedPixels = null;
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

                if (EventObserver.instance.happeningEvent == Events.DragMultiplePixels)
                {
                    if (!_cachedSelectedPixels.Any())
                        return;
                    targetPosition = targetPosition.Snap2();
                    var distanceDelta = targetPosition - _anchorMovePoint;
                    var firstOuterGroupSelected = new List<int>();
                    _cachedSelectedPixels.ForEach(pixel =>
                    {
                        var firstGroup = pixel.GetFirstGroup();
                        if (firstGroup.IsNotNull())
                        {
                            if (firstOuterGroupSelected.Contains(firstGroup.id))
                                return;
                            firstOuterGroupSelected.Add(firstGroup.id);
                            var firstGroupTransform = firstGroup.transform;
                            var firstGroupPosition = firstGroupTransform.position;
                            var snapPosition = firstGroupPosition + new Vector3(distanceDelta.x, distanceDelta.y, firstGroupPosition.z);
                            firstGroupTransform.position = snapPosition.ToVector2().Snap2();
                        }
                        else
                        {
                            var pixelTransform = pixel.transform;
                            var pixelPosition = pixelTransform.position;
                            var snapPosition = pixelPosition + new Vector3(distanceDelta.x, distanceDelta.y, pixelPosition.z);
                            pixelTransform.position = snapPosition.ToVector2().Snap2();
                        }
                    });
                    firstOuterGroupSelected.Clear();
                    _anchorMovePoint = targetPosition;
                }
                else
                {
                    var realPosition = targetPosition + _anchorMovePoint;
                    realPosition = realPosition.Snap2();
                    if (group.IsNotNull())
                    {
                        // if pixel has been in a group, then move that group
                        var group = GetFirstGroup();
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
                        onDrag.Invoke(this, realPosition);
                    }
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
            // Group.UngroupOneByOne();
            if(_cachedSelectedPixels.IsNotNull())
                _cachedSelectedPixels.Clear();
        }
        else
        {
            // var closestPixel = GetClosestPixel();
            // if (closestPixel.IsNotNull())
            // {
            //     var closestAnchor = GetClosestAnchor(closestPixel);
            //     if (closestAnchor.IsNotNull())
            //     {
            //         transform.position = closestAnchor.transform.position;
            //     }
            // }
        }
        if (onDrop.IsNotNull())
        {
            onDrop.Invoke(this, transform.position);
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

    public bool HasScript()
    {
        var host = !group.IsNotNull()
            ? GetComponent<ScriptableHost>()
            : GetFirstGroup().GetComponent<ScriptableHost>();
        return host.GetAllScripts().Any();
    }

    public ScriptableHost GetScriptableHost()
    {
        var host = !group.IsNotNull()
            ? GetComponent<ScriptableHost>()
            : GetFirstGroup().GetComponent<ScriptableHost>();
        return host;
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public Pixel GetClosestPixel()
    {
        // var bestPotential = TransformUtility.FindClosestObjectsOfType<Pixel>(transform.position, Constants.CLOSEST_PIXEL_DISTANCE, x => x != this);
        var bestPotential = pixelManager.FindClosestPixel(transform.position, Constants.CLOSEST_PIXEL_DISTANCE, x => x != this);
        return bestPotential;
    }

    public Transform GetClosestAnchor(Pixel pixel)
    {
        var bestPotential = TransformUtility.FindClosestObjectsBySpecific<Transform>(transform.position, Constants.CLOSEST_ANCHOR_DISTANCE, pixel.anchors);
        return bestPotential;
    }

    public void AddToGroup(Group group)
    {
        _group = group;
        transform.SetParent(group.transform);
    }

    public void RemoveGroup()
    {
        _group = null;
        transform.parent = null;
    }

    public void Select()
    {
        selecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
        // pivot.gameObject.SetActive(!Group.HasGroup(this));
    }

    public void Deselect()
    {
        selecting = false;
        VisibleSelection(false);
        // if (EventObserver.instance.happeningEvent != Events.DragPivotStart)
        //     pivot.gameObject.SetActive(false);
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

    public void Remove()
    {
        var scriptHostOfPixel = GetComponent<ScriptableHost>();
        if (scriptHostOfPixel.IsNotNull())
        {
            scriptHostOfPixel.RemoveAllScript();
        }
        if (_group.IsNotNull())
        {
            // Group.UngroupSingle(pixel);
            var cachedGroup = _group;
            cachedGroup.RemovePixel(this);
            if (cachedGroup.pixels.Count == 1)
            {
                var lastPixel = cachedGroup.pixels.First();
                cachedGroup.RemovePixel(lastPixel);
                cachedGroup.Remove();
            }
        }
        pixelManager.RemovePixel(id);
        DestroyImmediate(gameObject);
    }

    public GameObject Prefabricate(GameObject patternObject, Transform prefabContainer)
    {
        return Instantiate(patternObject, Vector3.zero, Quaternion.identity, prefabContainer);
    }

    public GameObject Unprefabricate(GameObject patternObject)
    {
        var instanceGo = Instantiate(patternObject, Vector3.zero, Quaternion.identity);
        var singlePixel = instanceGo.GetComponent<Pixel>();
        pixelManager.AddPixel(singlePixel);
        return instanceGo;
    }

    public void SetEnabledPivot(bool enabled)
    {
        pivot.gameObject.SetActive(enabled);
    }

    public void TogglePivot()
    {
        var pivotGo = pivot.gameObject;
        var activeSelf = pivotGo.activeSelf;
        pivot.gameObject.SetActive(!activeSelf);
    }

    public IEnumerable<Group> GetGroups()
    {
        if (group.IsNull())
            return null;
        var groupOfPixel = group;
        var parentalGroups = new List<Group>();
        if (groupOfPixel.IsNull())
            return parentalGroups;
        parentalGroups.Add(groupOfPixel);
        groupOfPixel.GetParentalGroups(ref parentalGroups);
        return parentalGroups;
    }

    public Group GetFirstGroup()
    {
        if (group.IsNull())
            return null;
        var groupsOfPixel = GetGroups();
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.Last();
    }

    public Group GetLastGroup()
    {
        if (group.IsNull())
            return null;
        var groupsOfPixel = GetGroups();
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.First();
    }

    public Group GetGroupAtIndex(int index)
    {
        if (group.IsNull())
            return null;
        var groupsOfPixel = GetGroups();
        if (!groupsOfPixel.Any())
            return null;
        return groupsOfPixel.ElementAt(groupsOfPixel.Count() - 1 - index);
    }

    public IEnumerable<Pixel> GetPixelsInGroup()
    {
        var groupOfPixel = GetFirstGroup();
        var pixelsInGroup = groupOfPixel.GetPixelsInChildren();
        return pixelsInGroup;
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