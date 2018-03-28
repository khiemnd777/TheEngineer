using UnityEngine;
using UnityEngine.UI.Extensions;

public class BlueprintConnector : MonoBehaviour
{
    public static BlueprintConnector current;

    public RectTransform headA;
    public RectTransform headB;
    public UILineRenderer lineRenderer;

    [System.NonSerialized]
    public BlueprintEntity a;
    [System.NonSerialized]
    public BlueprintEntity b;

    [System.NonSerialized]
    public BlueprintEntityPin anchorA;
    [System.NonSerialized]
    public BlueprintEntityPin anchorB;

    RectTransform _connectorOverlay;
    int _flagShownConnectorLine;

    Transform _anchorATransform;
    Transform _anchorBTransform;
    RectTransform _rectA;
    RectTransform _rectB;
    RectTransform _rectParentA;
    RectTransform _rectParentB;

    void Start()
    {
        
    }

    void Update()
    {
        // lineRenderer.gameObject.SetActive(anchorA != null && anchorB != null);
        // detect position of A and snag into head of A
        if(headA != null && !headA.Equals(null) && anchorA != null)
        {
            headA.position = _anchorATransform.position;
            var realAnchoredPositionA = ComputeRealAnchoredPosition(_rectA, _rectParentA, anchorA.pinType);
            lineRenderer.Points[0] = realAnchoredPositionA;
        }
        // detect position of B and snag into head of B
        if(headB != null && !headB.Equals(null) && anchorB != null)
        {
            headB.position = _anchorBTransform.position;
            var realAnchoredPositionB = ComputeRealAnchoredPosition(_rectB, _rectParentB, anchorB.pinType);
            lineRenderer.Points[1] = realAnchoredPositionB;
        }
        lineRenderer.OnRebuildRequested();
    }

    Vector2 ComputeRealAnchoredPosition(RectTransform rect, RectTransform rectParent, BlueprintEntityPinType pinType){
        var direction = pinType == BlueprintEntityPinType.In ? -1 : 1;
        var rect_anchoredPosition = rect.anchoredPosition;
        var rectParent_anchoredPosition = rectParent.anchoredPosition;
        var widthParent = rectParent.sizeDelta.x;
        var anchor_realAnchoredPositionX = rectParent_anchoredPosition.x + widthParent * rectParent.pivot.x * direction;
        var anchor_realAnchoredPositionY = rectParent_anchoredPosition.y + rect_anchoredPosition.y;
        var realAnchoredPosition = new Vector2(anchor_realAnchoredPositionX, anchor_realAnchoredPositionY);
        return realAnchoredPosition;
    }

    public void SetEntityA(BlueprintEntity a, BlueprintEntityPin anchorA)
    {
        this.a = a;
        this.anchorA = anchorA;
        _anchorATransform = anchorA.transform;
        _rectA = anchorA.GetComponent<RectTransform>();
        _rectParentA = _anchorATransform.parent.GetComponent<RectTransform>();
    }

    public void SetEntityB(BlueprintEntity b, BlueprintEntityPin anchorB)
    {
        this.b = b;
        this.anchorB = anchorB;
        _anchorBTransform = anchorB.transform;
        _rectB = anchorB.GetComponent<RectTransform>();
        _rectParentB = _anchorBTransform.parent.GetComponent<RectTransform>();
    }

    public void CreateConnectorOverlay()
    {
        var connectorGo = new GameObject("ConnectorOverlay");
        connectorGo.transform.SetParent(transform.parent);
        connectorGo.AddComponent<RectTransform>();
        _connectorOverlay = connectorGo.GetComponent<RectTransform>();
        _connectorOverlay.sizeDelta = Vector2.zero;
        _connectorOverlay.anchorMin = new Vector2(0, 1);
        _connectorOverlay.anchorMax = new Vector2(0, 1);
        connectorGo.transform.position = Vector3.zero;
    }

    public void DrawEndLineByMouse(BlueprintEntityPinType pinType)
    {
        if(_connectorOverlay == null || _connectorOverlay.Equals(null))
            return;
        _connectorOverlay.transform.position = Input.mousePosition;
        var connector = BlueprintConnector.current;
        if (connector != null)
        {
            var connectorLineRenderer = connector.lineRenderer;
            if(pinType == BlueprintEntityPinType.Out)
                connectorLineRenderer.Points[1] = _connectorOverlay.anchoredPosition;
            else
                connectorLineRenderer.Points[0] = _connectorOverlay.anchoredPosition;
            connectorLineRenderer.OnRebuildRequested();
            if(_flagShownConnectorLine == 2)
                connectorLineRenderer.gameObject.SetActive(true);
            else
                ++_flagShownConnectorLine;
        }
    }

    public void OnPinDrag(BlueprintEntityPin pin)
    {
        DrawEndLineByMouse(pin.pinType);
    }

    public void OnPinDragEnd(BlueprintEntityPin pin)
    {
        Destroy(_connectorOverlay.gameObject);
        _connectorOverlay = null;
    }
}