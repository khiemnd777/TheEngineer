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
    public bool selecting;

    public ExpandoObject pythonPixel;

    List<Scriptable> scriptableList;

    void Start(){
        StartCoroutine(GetPosition());

        text.text = name;
        // instance python's scriptable object
        pythonPixel = new ExpandoObject();
        ExpandoObjectUtility.SetVariable(pythonPixel, "id", GetInstanceID());
        ExpandoObjectUtility.SetVariable(pythonPixel, "position", new Position
        {
            x = transform.position.x,
            y = transform.position.y
        });
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

    public void Select(){
        selecting = true;
        VisibleSelection(true);
        VisibleHoverable(false);
    }

    public void Deselect(){
        selecting = false;
        VisibleSelection(false);
    }

    void VisibleHoverable(bool visible){
        if(!selecting)
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

    IEnumerator GetPosition()
    {
        while (!gameObject.IsNull())
        {
            yield return null;
            if (!gameObject.activeSelf)
                continue;
            var position = (Position)ExpandoObjectUtility.GetVariable(pythonPixel, "position");
            if (position.x == transform.position.x && position.y == transform.position.y)
                continue;
            transform.position = new Vector3(position.x, position.y, 0f);
        }
    }

}