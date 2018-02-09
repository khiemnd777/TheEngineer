using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScriptableHost : MonoBehaviour
{
    // public Transform container;
    public Transform includedScript;

    List<Scriptable> _scripts;

    public List<Scriptable> scripts
    {
        get
        {
            return _scripts ?? (_scripts = new List<Scriptable>());
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