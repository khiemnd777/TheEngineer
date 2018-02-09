using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DocumentEvents : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // hide script panel
        // ScriptManager.instance.ClearScriptable();
        // ScriptManager.instance.SetActivePanel(false);
    }
}