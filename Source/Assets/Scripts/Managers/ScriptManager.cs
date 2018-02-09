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

    [Header("Unimplemented script")]
    public RectTransform unimplementedScriptPanel;
    public InputField unimplementedScriptText;
    public InputField unimplementedScriptName;
    public Button unimplementedSaveButton;
    [Header("Implemented script")]
    public RectTransform implementedScriptPanel;
    public InputField implementedScriptText;
    public InputField unimplementedHostName;
    public Button implementedSaveButton;
    public Button implementedExecutedButton;
    public Button implementedStoppedButton;

    Scriptable scriptable;
}