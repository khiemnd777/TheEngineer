using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

[RequireComponent(typeof(Rigidbody2D))]
public class Scriptable : MonoBehaviour
{
    string script;
    bool stoppable;
    ScriptEngine engine;
    ScriptScope scope;
    ScriptRuntime runtime;
    
    void Start()
    {
        stoppable = true;
        engine = UnityPython.CreateEngine();

        StartCoroutine(GetPosition());
    }

    void Update()
    {
        if (stoppable)
            return;
        try
        {
            System.Action updateMethod;
            if (scope.TryGetVariable<System.Action>("update", out updateMethod))
            {
                if (updateMethod != null)
                {
                    updateMethod.Invoke();
                    updateMethod = null;
                }
            }
        }
        catch
        {
            // if source executes out any error.
        }
    }

    void FixedUpdate()
    {
        if (stoppable)
            return;
        try
        {
            System.Action fixedUpdateMethod;
            if (scope.TryGetVariable<System.Action>("fixed_update", out fixedUpdateMethod))
            {
                if (fixedUpdateMethod != null)
                {
                    fixedUpdateMethod.Invoke();
                    fixedUpdateMethod = null;
                }
            }
        }
        catch
        {
            // if source executes out any error.
        }
    }

    void OnMouseUp()
    {
        if(Input.GetMouseButtonUp(0)){
            ScriptManager.instance.SetScriptable(this, script);
            ScriptManager.instance.SetActivePanel(true);
        }
    }

    public void SetScript(string script)
    {
        this.script = script;
    }

    public void ExecuteScript()
    {
        runtime = engine.Runtime;
        scope = runtime.CreateScope();

        // Set position
        scope.SetVariable("position", new Position
        {
            x = transform.position.x,
            y = transform.position.y
        });
        var scriptContent = ScriptManager.instance.GetScriptContent();
        var source = engine.CreateScriptSourceFromString(scriptContent);
        IncludedTime.Include(scope);
        try
        {
            source.Execute(scope);
        }
        catch (System.Exception exc)
        {
            Debug.LogError(exc);
        }
        runtime.Shutdown();
        this.stoppable = false;
    }

    public void SetStoppable(bool stoppable)
    {
        this.stoppable = stoppable;
        if(stoppable)
            if(!runtime.IsNull())
                runtime.Shutdown();
    }

    IEnumerator GetPosition()
    {
        while (gameObject != null)
        {
            yield return null;
            if (!gameObject.activeSelf)
                continue;
            if (stoppable)
                continue;
            if (scope == null)
                continue;
            var position = scope.GetVariable<Position>("position");
            transform.position = new Vector3(position.x, position.y, 0f);
        }
    }
}

public class IncludedTime
{
    public static void Include(ScriptScope scope)
    {
        scope.SetVariable("delta_time", Time.deltaTime);
        scope.SetVariable("fixed_delta_time", Time.fixedDeltaTime);
        scope.SetVariable("fixed_time", Time.fixedTime);
        scope.SetVariable("frame_count", Time.frameCount);
        scope.SetVariable("time", Time.time);
        scope.SetVariable("time_scale", Time.timeScale);
    }
}