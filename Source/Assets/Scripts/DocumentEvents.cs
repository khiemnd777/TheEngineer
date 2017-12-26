using UnityEngine;

public class DocumentEvents : MonoBehaviour
{
    public void DocumentEventClick()
    {
        ScriptManager.instance.ClearScriptable();
        ScriptManager.instance.SetActivePanel(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
            }
        }
    }
}