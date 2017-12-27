using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentEvents : MonoBehaviour, IPointerDownHandler
{
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    //         if (hit.collider != null)
    //         {
    //             Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
    //         }
    //         else
    //         {
    //             // hide script panel
    //             ScriptManager.instance.ClearScriptable();
    //             ScriptManager.instance.SetActivePanel(false);
    //         }
    //     }
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        // hide script panel
        ScriptManager.instance.ClearScriptable();
        ScriptManager.instance.SetActivePanel(false);
    }
}