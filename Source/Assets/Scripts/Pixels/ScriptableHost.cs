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
        var instanceOfScript = Instantiate<Scriptable>(script, Vector3.zero, Quaternion.identity);
        instanceOfScript.name = script.name;
        instanceOfScript.host = this;
        instanceOfScript.transform.SetParent(container);
        _scripts.Add(instanceOfScript);
    }

    public void RemoveScript(Scriptable script)
    {
        _scripts.Remove(script);
        Destroy(script.gameObject);
    }

    public IEnumerable<Scriptable> GetAllScripts()
    {
        return _scripts;
    }
}