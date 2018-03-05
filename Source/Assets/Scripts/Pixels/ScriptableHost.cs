using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScriptableHost : MonoBehaviour
{
    // public Transform container;
    public Transform includedScript;
    public bool stoppable;

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
        if(!stoppable)
            return;
    }

    void FixedUpdate()
    {
        if(!stoppable)
            return;
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