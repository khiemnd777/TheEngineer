using UnityEngine;
using UnityEngine.UI;
using System.Dynamic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ContextMenu : MonoBehaviour
{
    public Canvas contextMenuCanvas;
    public Button itemPrefab;

    Dictionary<string, ExpandoObject> menuItems;
    Canvas _currentContextMenuCanvas;

    void Start()
    {
        menuItems = new Dictionary<string, ExpandoObject>();
        Register();
    }

    void Update(){
        if(Input.GetMouseButtonUp(1)){
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if(hit.collider.gameObject.transform.GetInstanceID() == transform.GetInstanceID()){
                    Show();
                }
            }
        }
    }

    void Show()
    {
        var position = Camera.main.WorldToScreenPoint(transform.position);
        _currentContextMenuCanvas = Instantiate<Canvas>(contextMenuCanvas);
        var contextMenu = _currentContextMenuCanvas.GetComponentInChildren<Image>();
        contextMenu.transform.position = position;
        foreach(var menuItem in menuItems){
            var eo = menuItem.Value;
            var prefab = (Button) ExpandoObjectUtility.GetVariable(eo, "itemPrefab");
            Instantiate<Button>(prefab, Vector2.one, Quaternion.identity, contextMenu.transform);
            prefab = null;
            eo = null;
        }
        contextMenu = null;
    }

    void Hide()
    {
        if(_currentContextMenuCanvas)
            Destroy(_currentContextMenuCanvas.gameObject);
    }

    public virtual void Register()
    {

    }

    public void RegisterItem(string key, string name, Button itemPrefab, System.Action action)
    {
        var itemEO = !menuItems.ContainsKey(key) ? new ExpandoObject() : menuItems[key];
        if (!menuItems.ContainsKey(key))
        {
            menuItems.Add(key, itemEO);
        }
        ExpandoObjectUtility.SetVariable(itemEO, "name", name);
        ExpandoObjectUtility.SetVariable(itemEO, "action", action);
        ExpandoObjectUtility.SetVariable(itemEO, "itemPrefab", itemPrefab);
    }

    public void SetName(string key, string name)
    {
        if (!menuItems.ContainsKey(key))
            return;
        var itemEO = menuItems[key];
        ExpandoObjectUtility.SetVariable(itemEO, "name", name);
    }

    public void SetAction(string key, System.Action action)
    {
        if (!menuItems.ContainsKey(key))
            return;
        var itemEO = menuItems[key];
        ExpandoObjectUtility.SetVariable(itemEO, "action", action);
    }

    public void SetAction(string key, Button itemPrefab)
    {
        if (!menuItems.ContainsKey(key))
            return;
        var itemEO = menuItems[key];
        ExpandoObjectUtility.SetVariable(itemEO, "itemPrefab", itemPrefab);
    }
}