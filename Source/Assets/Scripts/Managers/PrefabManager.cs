using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PrefabManager : MonoBehaviour
{
    #region Singleton
    static PrefabManager _instance;

    public static PrefabManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<PrefabManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active PrefabManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public GameObject Create(GameObject patternObject)
    {
        if(patternObject.IsNull())
            return null;
        var prefabComp = GetPrefabricated(patternObject);
        if (prefabComp.IsNull())
            return null;
        var prefabContainer = GameObject.Find("/" + Constants.PREFAB_CONTAINER);
        if (prefabContainer.IsNull())
        {
            prefabContainer = new GameObject(Constants.PREFAB_CONTAINER);
        }
        GameObject prefabGo = null;
        if (patternObject.GetComponent<Group>().IsNotNull())
        {
            var groupPrefab = Resources.Load<GameObject>(Constants.GROUP_PREFAB);
            prefabGo = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity, prefabContainer.transform);
            var group = prefabGo.GetComponent<Group>();
            if (group.IsNotNull())
            {
                var patternGroup = patternObject.GetComponent<Group>();
                var groups = patternGroup.groupChildren;
                var pixelsInGroup = patternGroup.pixels;
                foreach (var pixel in pixelsInGroup)
                {
                    // PixelManager.instance.AddPixel(pixel);
                    var pixelTransform = pixel.transform;
                    var instancePixel = Instantiate(pixel, pixelTransform.position, pixelTransform.rotation);
                    group.AddPixel(instancePixel);
                }
                CloneGroup(group, groups);
            }
        }
        else
        {
            prefabGo = Instantiate(patternObject, Vector3.zero, Quaternion.identity, prefabContainer.transform);
        }

        prefabGo.name = patternObject.name;
        prefabComp = GetPrefabricated(prefabGo);
        prefabComp.isPrefab = true;

        prefabGo.SetActive(false);

        // If having ScriptableHost
        var patternScriptHost = patternObject.GetComponent<ScriptableHost>();
        var prefabScriptHost = prefabGo.GetComponent<ScriptableHost>();
        if(patternScriptHost.IsNotNull() && prefabScriptHost.IsNotNull())
        {
            var patternScripts = patternScriptHost.GetAllScripts();
            prefabScriptHost.ReassignScripts(patternScripts);
            patternScripts = null;
        }

        prefabContainer = null;
        prefabComp = null;
        
        return prefabGo;
    }

    void CloneGroup(Group group, IEnumerable<Group> groups)
    {
        foreach(var childGroup in groups)
        {
            var groupChildren = childGroup.groupChildren;
            if(groupChildren.Any())
            {
                CloneGroup(childGroup, groupChildren);
            }
            var pixels = childGroup.pixels.Select(x => Instantiate(x, x.transform.position, x.transform.rotation));
            var instanceGroup = Group.Create(pixels);
            group.AddChildGroup(instanceGroup);
        }
    }

    public GameObject Unprefab(GameObject patternObject)
    {
        if(patternObject.IsNull())
            return null;
        var prefabComp = GetPrefabricated(patternObject);
        if(prefabComp.IsNull())
            return null;
        var unprefabGo = Instantiate(patternObject, Vector3.zero, Quaternion.identity);
        unprefabGo.name = patternObject.name;
        prefabComp = GetPrefabricated(unprefabGo);
        prefabComp.isPrefab = false;

        unprefabGo.SetActive(true);

        // If having ScriptableHost
        var patternScriptHost = patternObject.GetComponent<ScriptableHost>();
        var prefabScriptHost = unprefabGo.GetComponent<ScriptableHost>();
        if(patternScriptHost.IsNotNull() && prefabScriptHost.IsNotNull())
        {
            var patternScripts = patternScriptHost.GetAllScripts().ToArray();
            foreach(var script in patternScripts)
            {
                prefabScriptHost.AddScript(script);
            }
            patternScripts = null;
        }
        // If having Group
        var group = unprefabGo.GetComponent<Group>();
        if(group.IsNotNull())
        {
            var pixelsInGroup = group.GetComponentsInChildren<Pixel>();
            foreach(var pixel in pixelsInGroup)
            {
                PixelManager.instance.AddPixel(pixel);
            }
        }
        // if having Pixel
        var singlePixel = unprefabGo.GetComponent<Pixel>();
        if(singlePixel.IsNotNull())
        {
            PixelManager.instance.AddPixel(singlePixel);
        }

        prefabComp = null;
        return unprefabGo;
    }

    public GameObject Unprefab(GameObject patternObject, Vector3 unprefabPosition)
    {
        if(patternObject.IsNull())
            return null;
        var unprefabGo = Unprefab(patternObject);
        unprefabGo.transform.position = unprefabPosition.ToVector2().Snap2();
        return unprefabGo;
    }

    public IPrefabricated GetPrefabricated(GameObject target)
    {
        if(target.IsNull())
            return null;
        var prefabComp = target.GetComponent(typeof(IPrefabricated)) as IPrefabricated;
        return prefabComp;
    }
}