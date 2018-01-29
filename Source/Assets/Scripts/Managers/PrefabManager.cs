using UnityEngine;

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

        prefabComp = null;
        return unprefabGo;
    }

    public GameObject Unprefab(GameObject patternObject, Vector3 unprefabPosition)
    {
        if(patternObject.IsNull())
            return null;
        var unprefabGo = Unprefab(patternObject);
        unprefabGo.transform.position = unprefabPosition;
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