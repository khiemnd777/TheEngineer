using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public class Pixel : MonoBehaviour
{
    public TextMesh text;
    public Transform selection;
    public Transform hoverable;
    public Transform colliderGroup;
    public bool selecting;
    public bool tempSelecting;
    public bool dragMove;

    public ExpandoObject pythonPixel;

    Vector2 _anchorMovePoint;

    List<Scriptable> scriptableList;

    void Start(){
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
        if(Input.GetMouseButton(0)){
            DragMove();
        }
    }

    void DragMove(){
        if(dragMove){
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                // move if mouse has any movement
                var mousePosition = Input.mousePosition;
                var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                var targetPosition = new Vector2(worldMousePosition.x, worldMousePosition.y);
                transform.position = targetPosition + _anchorMovePoint;

                // var closestPixel = GetClosestPixel();
                // if(closestPixel.IsNotNull()){
                    
                //     closestPixel = null;
                // }
            }
        }
    }

    void OnMouseDown(){
        _anchorMovePoint = transform.localPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragMove = true;
    }

    void OnMouseUp(){
        dragMove = false;
    }

    void OnMouseOver(){
        if(selecting)
            return;
        VisibleHoverable(true);
    }

    void OnMouseExit(){
        if(selecting)
            return;
        VisibleHoverable(false);
    }

    public Pixel GetClosestPixel(){
        var pixels = FindObjectsOfType<Pixel>();
        Pixel bestTarget = null;
        var closestDistanceSqr = 1.5f;
        var currentPosition = transform.position;
        foreach(var potentialTarget in pixels){
            if(potentialTarget == this)
                continue;
            var directionToTarget = potentialTarget.transform.position - currentPosition;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr){
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    public void Select(){
        selecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
    }

    public void Deselect(){
        selecting = false;
        VisibleSelection(false);
    }

    public void SelectTemp(){
        tempSelecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
    }

    public void DeselectTemp(){
        tempSelecting = false;
        VisibleSelection(false);
    }

    void VisibleHoverable(bool visible){
        if(!selecting && !tempSelecting)
            text.gameObject.SetActive(visible);
        hoverable.gameObject.SetActive(visible);
    }

    void VisibleSelection(bool visible){
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
            ExpandoObjectUtility.SetVariable(pythonPixel, "position", new Position{
                x = transform.position.x,
                y = transform.position.y
            });
        }
    }
}