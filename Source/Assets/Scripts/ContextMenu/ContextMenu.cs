using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Dynamic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ContextMenu : MonoBehaviour
{
    #region Singleton
    static ContextMenu _instance;

    public static ContextMenu instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<ContextMenu>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active ContextMenu script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    [System.NonSerialized]
    public ContextMenuRegistrar target;
    public Button itemPrefab;

    Canvas _currentContextMenuCanvas;

    void Start()
    {
        if(FindObjectOfType<MouseEventDetector>() != null)
            MouseEventDetector.instance.onSingleMouseUp += (position) =>
            {
                HideOnMouseUp();
            };
        if(FindObjectOfType<SelectObjectManager>() != null)
            SelectObjectManager.instance.onMultipleSelecting += () =>
            {
                Hide();
            };
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
            {
                EventObserver.instance.happeningEvent = Events.HideContextMenu;
            }
            ContextMenuRegistrar registrar = null;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                foreach(var hit in raycastResults)
                {
                    var contextMenuRegistrar = hit.gameObject.GetComponent<ContextMenuRegistrar>();
                    if(contextMenuRegistrar.IsNotNull()){
                        registrar = contextMenuRegistrar;
                        contextMenuRegistrar = null;
                        break;
                    }
                }
            }
            else
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    registrar = hit.collider.GetComponent<ContextMenuRegistrar>();
                }
            }
            if (registrar.IsNotNull())
            {
                // if (target.IsNotNull() && target.GetID() == registrar.GetID())
                // {
                //     registrar = null;
                //     Hide();
                // }
                // else
                // {
                //     Hide();
                // }
                Hide();
                target = registrar;
                Show();
                registrar = null;
                return;
            }
            else
            {
                Hide();
            }
            Hide();
        }
        if (Input.GetMouseButton(2))
        {
            EventObserver.instance.happeningEvent = Events.None;
            Hide();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
                EventObserver.instance.happeningEvent = Events.HideContextMenu;
            Hide();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
                EventObserver.instance.happeningEvent = Events.HideContextMenu;
            Hide();
        }
    }

    void HideOnMouseUp()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        //     return;
        if (EventObserver.instance.happeningEvent == Events.HideContextMenu)
        {
            EventObserver.instance.happeningEvent = Events.None;
            return;
        }
        if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
        {
            EventObserver.instance.happeningEvent = Events.HideContextMenu;
        }

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.transform.GetInstanceID() != transform.GetInstanceID())
            {
                Hide();
                return;
            }
        }
        else
        {
            Hide();
        }
    }

    void Show()
    {
        if (target.IsNull())
            return;
        if(target.GetItems().Count == 0)
        {
            Hide();
            return;
        }
        EventObserver.instance.happeningEvent = Events.ShowContextMenu;
        // var position = target.HasRectTransform()
        //     ? target.transform.position
        //     : Camera.main.WorldToScreenPoint(target.transform.position);
        var mousePosition = Input.mousePosition;
        // var anchorPosition = position - mousePosition;
        var contextMenuCanvas = Resources.Load<Canvas>(Constants.CONTEXT_MENU_PREFAB);
        _currentContextMenuCanvas = Instantiate<Canvas>(contextMenuCanvas);
        var contextMenu = _currentContextMenuCanvas.GetComponentInChildren<Image>();
        contextMenu.transform.position = mousePosition;
        target.OnBeforeShow();
        foreach (var menuItem in target.GetItems())
        {
            var eo = menuItem.Value;
            var prefab = (Button)ExpandoObjectUtility.GetVariable(eo, "itemPrefab");
            var item = Instantiate<Button>(prefab, Vector2.one, Quaternion.identity, contextMenu.transform);
            var name = (string)ExpandoObjectUtility.GetVariable(eo, "name");
            var action = (System.Action)ExpandoObjectUtility.GetVariable(eo, "action");
            var text = item.GetComponentInChildren<Text>();
            item.onClick.RemoveAllListeners();
            item.onClick.AddListener(() =>
            {
                action.Invoke();
                if (EventObserver.instance.happeningEvent == Events.ShowContextMenu)
                    EventObserver.instance.happeningEvent = Events.HideContextMenu;
                Hide();
            });
            text.text = name;
            item.name = name;
            text = null;
            item = null;
            prefab = null;
            eo = null;
        }
        target.OnAfterShow();
        contextMenu = null;
    }

    void Hide()
    {
        if (_currentContextMenuCanvas.IsNotNull())
        {
            Destroy(_currentContextMenuCanvas.gameObject);
        }
        target = null;
    }
}