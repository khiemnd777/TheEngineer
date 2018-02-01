using UnityEngine;
using System.Linq;

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
        var scriptContainer = GameObject.Find("/" + Constants.PREFAB_CONTAINER);
        if (scriptContainer.IsNull())
        {
            scriptContainer = new GameObject(Constants.PREFAB_CONTAINER);
        }
        var prefabGo = Instantiate(patternObject, Vector3.zero, Quaternion.identity, scriptContainer.transform);
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

        scriptContainer = null;
        prefabComp = null;
        
        return prefabGo;
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