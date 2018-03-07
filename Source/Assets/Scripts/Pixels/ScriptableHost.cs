using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

public class ScriptableHost : MonoBehaviour
{
    // public Transform container;
    public Transform includedScript;
    public bool stoppable;

    public ScriptEngine engine;
    public ScriptScope scope;
    public ScriptRuntime runtime;

    System.Action _update;
    System.Action _fixedUpdate;
    System.Action _onLeftMouseUp;
    System.Action _onRightMouseUp;
    System.Action _onLeftMouseDown;
    System.Action _onRightMouseDown;

    List<Scriptable> _scripts;

    ScriptManager scriptManager;

    public List<Scriptable> scripts
    {
        get
        {
            return _scripts ?? (_scripts = new List<Scriptable>());
        }
    }

    void Awake()
    {
        scriptManager = ScriptManager.instance;
    }

    void Start()
    {
        stoppable = true;
    }

    void Update()
    {
        if(stoppable)
            return;
        if(_update == null)
            _update = GetVariable<System.Action>("__update");
        _update.Invoke();
    }

    void FixedUpdate()
    {
        if(stoppable)
            return;
        if(_fixedUpdate == null)
            _fixedUpdate = GetVariable<System.Action>("__fixedupdate");
        _fixedUpdate.Invoke();
    }

    void OnMouseUp()
    {
        if(stoppable)
            return;
        if (Input.GetMouseButtonUp(0))
        {
            if(_onLeftMouseUp == null)
                _onLeftMouseUp = GetVariable<System.Action>("__onleftmouseup");
            _onLeftMouseUp.Invoke();
            // ExecuteFunc<System.Action>("__onleftmouseup", (act, args) =>
            // {
            //     act.Invoke();
            // });
        }

        if (Input.GetMouseButtonUp(1))
        {
            if(_onRightMouseUp == null)
                _onRightMouseUp = GetVariable<System.Action>("__onrightmouseup");
            _onRightMouseUp.Invoke();
            // ExecuteFunc<System.Action>("__onrightmouseup", (act, args) =>
            // {
            //     act.Invoke();
            // });
        }
    }

    void OnMouseDown()
    {
        if(stoppable)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if(_onLeftMouseDown == null)
                _onLeftMouseDown = GetVariable<System.Action>("__onleftmousedown");
            _onLeftMouseDown.Invoke();
            // ExecuteFunc<System.Action>("__onleftmousedown", (act, args) =>
            // {
            //     act.Invoke();
            // });
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(_onRightMouseDown == null)
                _onRightMouseDown = GetVariable<System.Action>("__onrightmousedown");
            _onRightMouseDown.Invoke();
            // ExecuteFunc<System.Action>("__onrightmousedown", (act, args) =>
            // {
            //     act.Invoke();
            // });
        }
    }

    public void ExecuteScript()
    {
        ReleaseAnyVariable();

        if(engine == null)
            engine = UnityPython.CreateEngine();
        runtime = engine.Runtime;
        scope = runtime.CreateScope();

        scripts.ForEach(script => {
            var scriptContent = script.GetScriptHeader();
            scriptContent += script.script;
            // create script source from content
            var source = engine.CreateScriptSourceFromString(scriptContent);
            try
            {
                // execute python script
                source.Execute(scope);
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc);
            }
        });

        runtime.Shutdown();
        this.stoppable = false;
    }

    public void ReleaseAnyVariable()
    {
        _fixedUpdate = null;
        _update = null;
        _onLeftMouseDown = null;
        _onLeftMouseUp = null;
        _onRightMouseDown = null;
        _onRightMouseUp = null;
    }

    public void SetStoppable(bool stoppable)
    {
        this.stoppable = stoppable;
        if (stoppable)
            if (!runtime.IsNull())
                runtime.Shutdown();
    }

    public TAction GetVariable<TAction>(string name){
        if(scope.IsNull())
            return default(TAction);
        if(!scope.ContainsVariable(name))
            return default(TAction);
        try
        {
            return scope.GetVariable<TAction>(name);
        }
        catch
        {
            // if source executes out any error.
            throw;
        }
    }

    public void AddScript(Scriptable script)
    {
        if(scripts.Count(x => x.IsNotNull()) == 1)
            return;
        script.AddHost(this);
        scripts.Add(script);
    }

    public void RemoveScript(Scriptable script)
    {
        scripts.Remove(script);
        script.RemoveHost(this);
    }

    public void RemoveAllScript(){
        var scripts = GetAllScripts().ToArray();
        foreach(var script in scripts)
        {
            RemoveScript(script);
        }
        scripts = null;
    }

    public IEnumerable<Scriptable> GetAllScripts()
    {
        _scripts = scripts.Where(x => x.IsNotNull()).ToList();
        return _scripts;
    }

    public void ReassignScripts(IEnumerable<Scriptable> scripts)
    {
        _scripts = scripts.ToList();
    }
}