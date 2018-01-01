using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public class Pixel : MonoBehaviour
{
    public pixel pythonPixel;

    List<Scriptable> scriptableList;

    public void AddScriptable(Scriptable scriptable)
    {
        if (scriptableList.IsNull())
            scriptableList = new List<Scriptable>();
        scriptable.pixel = this;
        scriptableList.Add(scriptable);
    }
}

// Wrapped Pixel object for Python
public class pixel
{
    public int id { get; set; }
    public int name { get; set; }
    public Position position { get; set; }
}