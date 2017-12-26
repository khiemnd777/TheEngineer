using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public class Scriptable : MonoBehaviour
{
    public InputField script;

    ScriptEngine engine;
    ScriptScope scope;
    // Use this for initialization
    void Start()
    {
        engine = UnityPython.CreateEngine();
        scope = engine.CreateScope();

        // var code = "from UnityEngine import *\n";
        scope.SetVariable("position", new Position
        {
            x = transform.position.x,
            y = transform.position.y
        });
        
        // var source = engine.CreateScriptSourceFromString(code);
        // source.Execute(scope);

        // Debug.Log(scope.GetVariable<string>("str"));
        StartCoroutine(GetPosition());
    }

    void Update(){
        var source = engine.CreateScriptSourceFromString(script.text);
        try{
            source.Execute(scope);
        }
        catch {
            // if source executes out any error.
        }
    }

    IEnumerator GetPosition()
    {
        while (gameObject != null)
        {
            yield return null;
            if(!gameObject.activeSelf){
                continue;
            }
            var position = scope.GetVariable<Position>("position");
            transform.position = new Vector3(position.x, position.y, 0f);
        }
    }
}

public class Position
{
    public float x { get; set; }
    public float y { get; set; }
}