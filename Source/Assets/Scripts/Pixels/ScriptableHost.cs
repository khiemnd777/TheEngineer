using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptableHost : MonoBehaviour
{
    public Transform container;
    public Transform includedScript;

    List<Scriptable> _scripts;

    void Start()
    {
        _scripts = new List<Scriptable>();
    }

    public void AddScript(Scriptable script)
    {
        script.AddHost(this);
        _scripts.Add(script);
    }

    public void RemoveScript(Scriptable script)
    {
        _scripts.Remove(script);
        script.RemoveHost(this);
    }

    public IEnumerable<Scriptable> GetAllScripts()
    {
        return _scripts;
    }
}