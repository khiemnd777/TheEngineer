using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptableContainer : MonoBehaviour
{
    public Transform container;
    public Transform includedScript;

    List<Scriptable> scripts;

    public virtual void Start()
    {
        scripts = new List<Scriptable>();
    }

    public void AddScript(Scriptable script)
    {
        scripts.Add(script);
    }

    public IEnumerable<Scriptable> GetAllScripts(){
        return scripts;
    }
}