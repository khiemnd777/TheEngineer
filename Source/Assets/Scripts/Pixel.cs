using System.Linq;
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
        scope.SetVariable("__create_pixel", new System.Action<string, float, float, string>((name, x, y, parentName) =>
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
        scope.SetVariable("__find_pixels", new System.Func<string, object>((name) =>
        {
            var objs = FindObjectsOfType<Pixel>();
            var objsWithName = objs.Where(go => name.Equals(go.name));
            var objsWrap = objsWithName.Select(go => {
                var dynObj = new System.Dynamic.ExpandoObject();
                dynObj.AddVariable("id", GetInstanceID());
                if(go.pythonVariables.Any()){
                    foreach(var pyVar in go.pythonVariables){
                        dynObj.AddVariable(pyVar.Key, pyVar.Value);
                    }
                }
                return dynObj;
            });
            
            return objsWrap.ToArray();
        }));
    }
}

// Wrapped Pixel object for Python
public class pixel : scriptable
{
    
}