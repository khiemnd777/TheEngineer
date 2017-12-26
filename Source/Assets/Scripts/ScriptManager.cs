using UnityEngine;
using UnityEngine.UI;

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

    Scriptable scriptable;

    public void SetScriptable(Scriptable scriptable)
    {
        this.scriptable = scriptable;
    }

    public void ClearScriptable(){
        scriptable = null;
    }

    public void Execute()
    {
        if(scriptable.IsNull())
            return;
        scriptable.SetScript(scriptText.text);
        scriptable.ExecuteScript();
    }

    public void Stop()
    {
        if(scriptable.IsNull())
            return;
        scriptable.SetStoppable(true);
    }

    public void SetActivePanel(bool active){
        scriptPanel.gameObject.SetActive(active);
    }
}