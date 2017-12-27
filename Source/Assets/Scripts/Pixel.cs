using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using UnityEngine.UI;
using IronPython;

public class Pixel : Scriptable
{
    public override void IncludeVariables()
    {
        base.IncludeVariables();

        // Create Pixel
        scope.SetVariable("create_pixel", new System.Action<string, float, float, string>((name, x, y, parentName) =>
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
        scope.SetVariable("find_pixels", new System.Func<string, object>((name) =>
        {
            var objs = FindObjectsOfType<Pixel>();
            var objsWithName = objs.Where(go => name.Equals(go.name));
            var objsWrap = objsWithName.Select(go => new pixel
            {
                id = go.GetInstanceID(),
                position = new Position
                {
                    x = go.transform.position.x,
                    y = go.transform.position.y
                },
            });
            return objsWrap.ToArray();
        }));
    }
}

// Wrapped Pixel object for Python
public class pixel
{
    public int id { get; set; }
    public Position position { get; set; }
}