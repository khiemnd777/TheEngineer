using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public class Pixel : Scriptable
{
    public override void IncludeUnityVariables()
    {
        base.IncludeUnityVariables();

        // Create Pixel
        scope.SetVariable("__createpixel", new System.Action<string, float, float, string>((name, x, y, parentName) =>
        {
            var pixelPrefab = Resources.Load<Pixel>(Constants.PIXEL_PREFAB);
            if (pixelPrefab.IsNull())
                return;

            var objs = GameObject.FindGameObjectsWithTag("Pixel");
            var objsWithName = objs.Where(go => name.Equals(go.name));
            Pixel pixelObj = null;
            if (objsWithName.Any())
            {
                var firstObjsWithName = objsWithName.FirstOrDefault();
                pixelObj = Instantiate(pixelPrefab, new Vector2(x, y), Quaternion.identity, firstObjsWithName.transform);

            }
            else
            {
                pixelObj = Instantiate(pixelPrefab, new Vector2(x, y), Quaternion.identity);
            }

            if (!pixelObj.IsNull())
            {
                pixelObj.name = name;
            }
        }));

        // Find Pixels
        scope.SetVariable("__findpixel", new System.Func<string, object>((name) =>
        {
            var objs = FindObjectsOfType<Pixel>();
            var objsWithName = objs.FirstOrDefault(go => name.Equals(go.name));
            if(objsWithName.IsNull())
                return null;
            return objsWithName.pythonScriptable;
        }));
    }
}

// Wrapped Pixel object for Python
public class pixel : scriptable
{
    
}