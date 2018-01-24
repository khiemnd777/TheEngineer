using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyManager : MonoBehaviour
{
    #region Singleton
    static HierarchyManager _instance;

    public static HierarchyManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<HierarchyManager>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active HierarchyManager script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public HierarchyItem itemPrefab;
    public RectTransform container;

    List<HierarchyItem> _items;

    public List<HierarchyItem> items
    {
        get
        {
            return _items ?? (_items = new List<HierarchyItem>());
        }
    }

    void Start(){
        var scripts = Create("Scripts");
            Create("instance-script.py", null, scripts);
            Create("instance-script.py", null, scripts);
            Create("instance-script.py", null, scripts);
        var pixels = Create("Pixels");
            Create("Pixel 1", null, pixels);
            var groups = Create("Group 1", null, pixels);
                Create("Pixel 2", null, groups);
                Create("Pixel 3", null, groups);
    }

    void Update()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            // just get pixel object
            var hittingItem = hit.transform.GetComponent<HierarchyItem>();
            if (hittingItem.IsNotNull())
            {
                Debug.Log(hittingItem);
                hittingItem = null;
            }
        }
    }

    public HierarchyItem Create(string name, GameObject reference = null, HierarchyItem parent = null)
	{
		var itemFromResource = Resources.Load<HierarchyItem>(Constants.HIERARCHY_ITEM_PREFAB);
		var instanceItem = Instantiate<HierarchyItem>(itemFromResource, Vector3.zero, Quaternion.identity);
        var textItem = instanceItem.GetComponentInChildren<Text>();
        textItem.text = name;
		instanceItem.name = name;
		instanceItem.reference = reference;
		if(parent.IsNotNull()){
			instanceItem.SetParent(parent);
            var textPosition = parent.GetComponentInChildren<Text>().transform.localPosition;
            var x = textPosition.x + 35f;
            var pos = instanceItem.GetComponentInChildren<Text>().transform.localPosition;
            instanceItem.GetComponentInChildren<Text>().transform.localPosition = new Vector3(x, pos.y, pos.z);
		}
		// item contains into container
		instanceItem.transform.SetParent(container.transform);
        instanceItem.transform.localScale = Vector3.one;
        items.Add(instanceItem);
        return instanceItem;
	}

    public void SetParent(HierarchyItem parent)
    {

    }
}
