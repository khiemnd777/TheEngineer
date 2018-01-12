using UnityEngine;
using UnityEngine.UI;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public class ContextMenuRegistrar : MonoBehaviour
{
    Dictionary<string, ExpandoObject> menuItems;

    void Start()
    {
        menuItems = new Dictionary<string, ExpandoObject>();
        Register();
    }

    public virtual void Register()
    {
        
    }

    public Dictionary<string, ExpandoObject> GetItems(){
        return menuItems;
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