using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

public class ScriptManager : MonoBehaviour
{
    #region Singleton
    static ScriptManager _instance;

    public static ScriptManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<ScriptManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active ScriptManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public RectTransform scriptPanel;
    public InputField scriptText;
    public Text title;
    [Space]
    public Scriptable scriptablePrefab;

    Scriptable scriptable;

    public void SetScriptable(Scriptable scriptable, string scriptContent = "")
    {
        this.scriptable = scriptable;
        title.text = scriptable.name;
        SetScriptContent(scriptContent);
    }

    public void ClearScriptable()
    {
        scriptable = null;
        scriptText.text = string.Empty;
    }

    public void SetScriptContent(string content)
    {
        scriptText.text = content;
    }

    public string GetScriptContent()
    {
        return scriptText.text;
    }

    public void Save()
    {
        if (scriptable.IsNull())
            return;
        scriptable.SetScript(scriptText.text);
    }

    public void Execute()
    {
        if (scriptable.IsNull())
            return;
        scriptable.SetScript(scriptText.text);
        scriptable.ExecuteScript();
    }

    public void Stop()
    {
        if (scriptable.IsNull())
            return;
        scriptable.SetStoppable(true);
    }

    public void SetActivePanel(bool active)
    {
        scriptPanel.gameObject.SetActive(active);
    }

    public bool ScriptableExists(string name)
    {
        var go = GameObject.Find("Scriptable/" + name);
        return !go.IsNull();
    }

    public void CreateScriptable()
    {
        // create scriptable
        var initPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var stored = GetStoredScriptable();
        var initial = Instantiate<Scriptable>(scriptablePrefab, initPosition, Quaternion.identity, stored);
        // then, set script content into panel
        SetScriptable(initial, initial.script);
        SetActivePanel(true);
        stored = null;
        initial = null;
    }

    Transform GetStoredScriptable()
    {
        // find stored scriptable
        var stored = GameObject.Find("Scriptable");
        if(stored.IsNull()){
            // if not exists, then immediately create it
            stored = new GameObject("Scriptable");
            stored.transform.position = Vector3.zero;
            stored.transform.localScale = Vector3.one;
            stored.transform.rotation = Quaternion.identity;
            return stored.transform;
        }
        return stored.transform;
    }
}