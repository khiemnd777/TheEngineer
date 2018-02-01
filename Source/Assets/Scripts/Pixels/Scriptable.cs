using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;

public class Scriptable : MonoBehaviour
{
    public string variableName;
    public string script;
    public bool stoppable;

    public ScriptEngine engine;
    public ScriptScope scope;
    public ScriptRuntime runtime;

    List<ScriptableHost> _hosts = new List<ScriptableHost>();

    public static Scriptable CreateInstance()
    {
        var res = Resources.Load<Scriptable>(Constants.SCRIPT_PREFAB);
        var instanceOfScript = Instantiate<Scriptable>(res, Vector3.zero, Quaternion.identity);
        var name = "Instanced Script";
        instanceOfScript.name = name;
        instanceOfScript.variableName = name.ToVariableName();

        // contains into script container
        var scriptContainer = GameObject.Find("/" + Constants.SCRIPT_CONTAINER);
        if (scriptContainer.IsNull())
        {
            scriptContainer = new GameObject(Constants.SCRIPT_CONTAINER);
        }
        instanceOfScript.transform.SetParent(scriptContainer.transform);
        // release memory
        scriptContainer = null;
        res = null;
        name = null;
        return instanceOfScript;
    }

    public static Scriptable CreateInstanceAndAssignTo(ScriptableHost host)
    {
        var instanceOfScript = CreateInstance();
        host.AddScript(instanceOfScript);
        return instanceOfScript;
        // Destroy(instanceOfScript.gameObject);
    }

    public static IEnumerable<Scriptable> FindByName(string name)
    {
        var scripts = FindObjectsOfType<Scriptable>();
        var findingOut = scripts.Where(x => name.Equals(x.name));
        scripts = null;
        return findingOut;
    }

    public static IEnumerable<Scriptable> FindByVariableName(string variableName)
    {
        var scripts = FindObjectsOfType<Scriptable>();
        var findingOut = scripts.Where(x => variableName.Equals(x.variableName));
        scripts = null;
        return findingOut;
    }

    void Start()
    {
        stoppable = true;
        engine = UnityPython.CreateEngine();

        StartCoroutine(UpdatingUnityVariables());
        StartCoroutine(SyncPythonVariablesAndUnityVariables());
    }

    void Update()
    {
        if (stoppable)
            return;
        ExecuteFunc<System.Action>("__update", (act, args) =>
        {
            act.Invoke();
            // StorePythonVariables();
        });
    }

    void FixedUpdate()
    {
        if (stoppable)
            return;
        ExecuteFunc<System.Action>("__fixedupdate", (act, args) =>
        {
            act.Invoke();
            // StorePythonVariables();
        });
    }

    void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ScriptManager.instance.SetScriptable(this, script);
            ScriptManager.instance.SetActivePanel(true);

            ExecuteFunc<System.Action>("__onleftmouseup", (act, args) =>
            {
                act.Invoke();
            });
        }

        if (Input.GetMouseButtonUp(1))
        {
            ExecuteFunc<System.Action>("__onrightmouseup", (act, args) =>
            {
                act.Invoke();
            });
        }
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ExecuteFunc<System.Action>("__onleftmousedown", (act, args) =>
            {
                act.Invoke();
            });
        }
        if (Input.GetMouseButtonDown(1))
        {
            ExecuteFunc<System.Action>("__onrightmousedown", (act, args) =>
            {
                act.Invoke();
            });
        }
    }

    public void AddHost(ScriptableHost host)
    {
        _hosts.Add(host);
    }

    public void RemoveHost(ScriptableHost host){
        _hosts.Remove(host);
    }

    public void SetName(string newName)
    {
        name = newName;
    }

    public void SetVariableName(string newVariableName)
    {
        if (Regex.IsMatch(newVariableName, @"\s+"))
        {
            newVariableName = Regex.Replace(newVariableName, @"\s+", "");
        }
        var existingScripts = FindByVariableName(newVariableName);
        if (existingScripts.Any())
        {
            newVariableName += "_clone";
        }
        variableName = newVariableName;
        existingScripts = null;
    }

    public void SetNameAndVariableName(string newName)
    {
        SetName(newName);
        var nameWithoutCaseSensitive = newName.ToLower();
        SetVariableName(nameWithoutCaseSensitive);
    }

    public void SetScript(string script)
    {
        this.script = script;
    }

    public void Remove()
    {
        var hosts = _hosts.ToArray();
        foreach(var host in hosts)
        {
            host.RemoveScript(this);
        }
        _hosts.Clear();
        hosts = null;
        DestroyImmediate(gameObject);
    }

    public void ExecuteScript()
    {
        runtime = engine.Runtime;
        scope = runtime.CreateScope();

        var scriptContent = GetScriptHeader();
        scriptContent += script;

        // create script source from content
        var source = engine.CreateScriptSourceFromString(scriptContent);

        // include unity variables
        IncludeVariables();
        // updated unity variables
        UpdateUnityVariables();
        // include python variables
        IncludePythonVariables();
        try
        {
            // execute python script
            source.Execute(scope);
            // store python variables
            StorePythonVariables();
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
        if (stoppable)
            if (!runtime.IsNull())
                runtime.Shutdown();
    }

    string GetScriptHeader()
    {
        var content = "#Generated by IronPython 2.7.7\n";
        content += "from UnityEngine import Debug as debug\n";
        return content;
    }

    public virtual void IncludeVariables()
    {
        // // Position of scriptable object
        // scope.SetVariable("position", ExpandoObjectUtility.GetVariable(pixel.pythonPixel, "position"));

        // // Create Pixel
        // scope.SetVariable("__create", new System.Action<string, float, float, string, string>((name, x, y, scriptableName, parentName) =>
        // {
        //     var pixelPrefab = Resources.Load<Pixel>(Constants.PIXEL_PREFAB);
        //     if (pixelPrefab.IsNull())
        //         return;

        //     var objs = GameObject.FindGameObjectsWithTag("Pixel");
        //     var objsWithName = objs.Where(go => name.Equals(go.name));
        //     Pixel pixelObj = null;
        //     if (objsWithName.Any())
        //     {
        //         var firstObjsWithName = objsWithName.FirstOrDefault();
        //         pixelObj = Instantiate(pixelPrefab, new Vector2(x, y), Quaternion.identity, firstObjsWithName.transform);

        //     }
        //     else
        //     {
        //         pixelObj = Instantiate(pixelPrefab, new Vector2(x, y), Quaternion.identity);
        //     }

        //     if (!pixelObj.IsNull())
        //     {
        //         pixelObj.name = name;
        //         // pixelObj.scriptable
        //         // Find scriptable from hierachy
        //         if (!string.IsNullOrEmpty(scriptableName))
        //         {
        //             var scriptableGO = GameObject.Find("Scriptable/" + scriptableName);
        //             if (!scriptableGO.IsNull())
        //             {
        //                 var scriptable = scriptableGO.GetComponent<Scriptable>();
        //                 pixelObj.AddScriptable(scriptable);
        //                 scriptable = null;
        //             }
        //             scriptableGO = null;
        //         }
        //         pixelObj = null;
        //     }
        //     pixelPrefab = null;
        // }));

        // // Find Pixels
        // scope.SetVariable("__find", new System.Func<string, object>((name) =>
        // {
        //     var objs = FindObjectsOfType<Pixel>();
        //     var objsWithName = objs.FirstOrDefault(go => name.Equals(go.name));
        //     objs = null;
        //     if (objsWithName.IsNull())
        //         return null;
        //     return objsWithName.pythonPixel;
        // }));

        // scope.SetVariable("__setparent", new System.Action<string>((parentName) => {
        //     var objs = FindObjectsOfType<Pixel>();
        //     var objWithName = objs.FirstOrDefault(go => parentName.Equals(go.name));
        //     objs = null;
        //     if(objWithName.IsNull())
        //         return;
        //     this.pixel.transform.SetParent(objWithName.transform);
        // }));
    }

    void UpdateUnityVariables()
    {
        // Time
        scope.SetVariable("__deltatime", Time.deltaTime);
        scope.SetVariable("__fixeddeltatime", Time.fixedDeltaTime);
        scope.SetVariable("__fixedtime", Time.fixedTime);
        scope.SetVariable("__framecount", Time.frameCount);
        scope.SetVariable("__time", Time.time);
        scope.SetVariable("__timescale", Time.timeScale);

        // Mouse position
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        scope.SetVariable("__mouseposition", new Position
        {
            x = mousePosition.x,
            y = mousePosition.y
        });
    }

    void ExecuteFunc<TAction>(string name, System.Action<TAction, object[]> action, params object[] args)
    {
        if (scope.IsNull())
            return;
        try
        {
            TAction act;
            if (scope.TryGetVariable<TAction>(name, out act))
            {
                if (act != null)
                {
                    action.Invoke(act, args);
                }
            }
        }
        catch
        {
            // if source executes out any error.
        }
    }

    void StorePythonVariables()
    {
        // if (scope.IsNull())
        //     return;
        // var items = scope.GetItems();
        // items = items.Where(x => !x.Key.StartsWith("__"));
        // foreach (var pyVar in items)
        // {
        //     ExpandoObjectUtility.SetVariable(pixel.pythonPixel, pyVar.Key, pyVar.Value);
        // }
    }

    void IncludePythonVariables()
    {
        // if (scope.IsNull())
        //     return;
        // var pythonVariables = pixel.pythonPixel as IDictionary<string, object>;
        // foreach (var pyVar in pythonVariables)
        // {
        //     scope.SetVariable(pyVar.Key, pyVar.Value);
        // }
    }

    IEnumerator SyncPythonVariablesAndUnityVariables()
    {
        while (!gameObject.IsNull())
        {
            yield return null;
            if (!gameObject.activeSelf)
                continue;
            if (stoppable)
                continue;
            if (scope.IsNull())
                continue;
            System.Action update;
            System.Action fixedUpdate;
            if (scope.TryGetVariable("__update", out update)
                || scope.TryGetVariable("__fixedupdate", out fixedUpdate))
            {
                StorePythonVariables();
            }
            update = null;
            fixedUpdate = null;
        }
    }

    IEnumerator UpdatingUnityVariables()
    {
        while (!gameObject.IsNull())
        {
            yield return null;
            if (stoppable)
                continue;
            if (scope.IsNull())
                continue;
            System.Action update;
            System.Action fixedUpdate;
            if (scope.TryGetVariable("__update", out update)
                || scope.TryGetVariable("__fixedupdate", out fixedUpdate))
            {
                UpdateUnityVariables();
            }
            update = null;
            fixedUpdate = null;
        }
    }
}