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

    public void Create(GameObject patternObject)
    {
        var scriptContainer = GameObject.Find("/Scripts");
        if (scriptContainer.IsNull())
        {
            scriptContainer = new GameObject("Scripts");
        }
        Instantiate(patternObject, Vector3.zero, Quaternion.identity, scriptContainer);
    }
}