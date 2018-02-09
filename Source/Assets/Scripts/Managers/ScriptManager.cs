using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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

    [Header("Unimplemented script")]
    public RectTransform unimplementedScriptPanel;
    public InputField unimplementedScriptText;
    public InputField unimplementedScriptName;
    public Button unimplementedSaveButton;
    [Header("Implemented script")]
    public RectTransform implementedScriptPanel;
    public Text implementedScriptName;
    public InputField implementedScriptText;
    public InputField implementedHostName;
    public Button implementedSaveButton;
    public Button implementedExecutedButton;
    public Button implementedStoppedButton;

    Scriptable currentScriptable;

    void Start()
    {
        VisibleUnimplementedScriptPanel(false);
        VisibleImplementedScriptPanel(false);

        // Unimplemented
        unimplementedSaveButton.onClick.AddListener(() => 
        {
            Debug.Log("Unimplemented saving button clicked.");
            SaveUnimplementedScript();
        });

        // Implemented
        implementedSaveButton.onClick.AddListener(() => 
        {
            Debug.Log("Implemented saving button clicked.");
            SaveImplementedScript();
        });

        implementedExecutedButton.onClick.AddListener(() => 
        {
            Debug.Log("Implemented executed button clicked.");
            currentScriptable.ExecuteScript();
        });

        implementedStoppedButton.onClick.AddListener(() => 
        {
            Debug.Log("Implemented stopped button clicked.");
        });
    }

    void SaveImplementedScript()
    {
        if(currentScriptable.IsNull())
            return;
        currentScriptable.script = implementedScriptText.text;
    }

    void SaveUnimplementedScript()
    {
        if(currentScriptable.IsNull())
            return;
        currentScriptable.script = unimplementedScriptText.text;
    }

    public void ShowUnimplementedScriptPanel(Scriptable scriptable)
    {
        VisibleImplementedScriptPanel(false);
        currentScriptable = scriptable;
        unimplementedScriptText.text = scriptable.script;
        unimplementedScriptName.text = scriptable.name;
        VisibleUnimplementedScriptPanel(true);
    }

    public void ShowImplementedScriptPanel(ScriptableHost host)
    {
        VisibleUnimplementedScriptPanel(false);
        currentScriptable = host.scripts.First();
        if(currentScriptable.IsNull()){
            VisibleImplementedScriptPanel(false);
            return;
        }
        implementedScriptText.text = currentScriptable.script;
        implementedScriptName.text = currentScriptable.name;
        implementedHostName.text = host.name;
        VisibleImplementedScriptPanel(true);
    }

    public void VisibleUnimplementedScriptPanel(bool visible)
    {
        unimplementedScriptPanel.gameObject.SetActive(visible);
    }

    public void VisibleImplementedScriptPanel(bool visible)
    {
        implementedScriptPanel.gameObject.SetActive(visible);
    }
}