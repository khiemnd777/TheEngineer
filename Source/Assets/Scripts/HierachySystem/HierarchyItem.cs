using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchyItem : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerUpHandler
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    public Transform arrow;
    public InputField inputField;
    public Button okButton;
    [System.NonSerialized]
    public HierarchyItem parent;
    [System.NonSerialized]
    public HierarchyItem originalParent;
    [System.NonSerialized]
    public int originalParentId;
    [System.NonSerialized]
    public GameObject reference;
    [System.NonSerialized]
    public bool pointerEntered;
    [System.NonSerialized]
    public bool expanded;
    [System.NonSerialized]
    public bool draggable;

    public OnDragEvent onDragEndEvent;

    bool dragging;

    HierarchyItem draggedInstance;

    int _id;

    public int id
    {
        get
        {
            return _id;
        }
    }

    public Text text
    {
        get
        {
            return GetComponentInChildren<Text>();
        }
    }

    public int siblingIndex
    {
        get
        {
            return transform.GetSiblingIndex();
        }
    }

    public Image arrowImage
    {
        get
        {
            return arrow.GetComponent<Image>();
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }

    public Canvas canvas
    {
        get
        {
            return GetComponentInParent<Canvas>();
        }
    }

    void Awake()
    {
        VisiableInputField(false);
    }

    void Start()
    {
        _id = this.GetID();
        okButton.onClick.AddListener(() => 
        {
            var inputFieldText = inputField.text;
            if(reference.IsNotNull() && !string.IsNullOrEmpty(inputFieldText)){
                reference.name = inputFieldText;
            }
            name = inputFieldText;
            text.text = inputFieldText;
            ToggleInputField();
        });
    }

    public void SetParent(HierarchyItem parent)
    {
        this.parent = parent;
        var textPosition = parent.text.transform.localPosition;
        var x = textPosition.x + Constants.HIERARCHY_ITEM_SPACE_LEVEL;
        var pos = text.transform.localPosition;
        text.transform.localPosition = new Vector3(x, pos.y, pos.z);
        parent.arrowImage.enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerEntered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragging)
            return;
        if (!Input.GetMouseButtonUp(0))
            return;
        if (HierarchyManager.instance.AnyChild(this.GetID()))
        {
            if (!expanded)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggedInstance.IsNotNull())
            Destroy(draggedInstance.gameObject);
        if (!draggable)
            return;
        draggedInstance = Instantiate<HierarchyItem>(this, eventData.position, Quaternion.identity, canvas.transform);
        draggedInstance.rectTransform.pivot = new Vector2(0f, 1f);
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable)
            return;
        if (draggedInstance.IsNotNull())
        {
            var mousePosition = Input.mousePosition;
            var instanceRect = draggedInstance.rectTransform.rect;
            var instancePosition = draggedInstance.transform.position;
            // var draggedPosition = new Vector3(mousePosition.x + instanceRect.width, mousePosition.y - instanceRect.height, instancePosition.z);
            var draggedPosition = new Vector3(mousePosition.x + 15f, mousePosition.y - 15f, instancePosition.z);
            draggedInstance.transform.position = draggedPosition;
            EventObserver.instance.happeningEvent = Events.DragHierarchyItem;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable)
            return;
        if (draggedInstance.IsNotNull())
        {
            if (onDragEndEvent.IsNotNull())
            {
                onDragEndEvent.Invoke(this, eventData.position);
            }
            Destroy(draggedInstance.gameObject);
        }
        dragging = false;
    }

    public void ToggleInputField()
    {
        var inputFieldGo = inputField.gameObject;
        var activeSelf = inputFieldGo.activeSelf;
        if(!activeSelf)
        {
            inputField.text = reference.IsNotNull() ? reference.name : string.Empty;
        }
        inputFieldGo.SetActive(!activeSelf);
    }

    public void VisiableInputField(bool visible)
    {
        var inputFieldGo = inputField.gameObject;
        inputFieldGo.SetActive(visible);
    }

    public void Collapse()
    {
        expanded = false;
        var children = HierarchyManager.instance.GetChildren(this.GetID());
        children.ForEach(x =>
        {
            x.Collapse();
            x.gameObject.SetActive(false);
        });
        arrowImage.sprite = HierarchyManager.instance.rightArrowSprite;
        children = null;
    }

    public void Expand()
    {
        expanded = true;
        var children = HierarchyManager.instance.GetChildren(this.GetID());
        children.ForEach(x =>
        {
            x.gameObject.SetActive(true);
        });
        arrowImage.sprite = HierarchyManager.instance.bottomArrowSprite;
        children = null;
    }
}
