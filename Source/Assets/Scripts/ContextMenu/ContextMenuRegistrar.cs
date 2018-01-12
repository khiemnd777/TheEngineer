using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public class ContextMenuRegistrar : MonoBehaviour
{
    protected Dictionary<string, ExpandoObject> menuItems;
    protected Dictionary<string, ExpandoObject> shownItems;

    protected virtual void Start()
    {
        menuItems = new Dictionary<string, ExpandoObject>();
        shownItems = new Dictionary<string, ExpandoObject>();
        Register();
    }

    public virtual void Register()
    {
        
    }

    public virtual void OnBeforeShow(){

    }

    public virtual void OnAfterShow(){
        shownItems.Clear();
    }

    public Dictionary<string, ExpandoObject> GetItems(){
        var items = !shownItems.Any() ? menuItems : shownItems;
        return items;
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

    public void RegisterItem(string key, string name, System.Action action)
    {
        RegisterItem(key, name, ContextMenu.instance.itemPrefab, action);
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

    public void SetItemPrefab(string key, Button itemPrefab)
    {
        if (!menuItems.ContainsKey(key))
            return;
        var itemEO = menuItems[key];
        ExpandoObjectUtility.SetVariable(itemEO, "itemPrefab", itemPrefab);
    }
}