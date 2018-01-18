using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptableTarget : MonoBehaviour
{
    List<Scriptable> scripts;

    public virtual void Start()
    {
        scripts = new List<Scriptable>();
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public void AddScript(Scriptable script)
    {
        scripts.Add(script);
    }
}