using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BlueprintEntityPinType
{
    In, Out
}

public class BlueprintEntityPin : MonoBehaviour
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
    , IPointerDownHandler
    , IPointerUpHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    const string connectorResPath = "Prefabs/Blueprint Behaviour/Blueprint Connector";

    [System.NonSerialized]
    public BlueprintEntity entity;
    public BlueprintEntityPinType pinType;

    public System.Action<BlockConnector> dropToConnectorCallback;

    Transform _parentTransform;
    BlueprintConnector _connectorPrefab;
    Image _pinImage;
    Color _highlightPin = new Color32(207, 255, 43, 255);
    Color _normalPin = new Color32(255, 255, 255, 255);
    RectTransform _connectorOverlay;
    int _flagShownConnectorLine = 0;

    void Start()
    {
        _parentTransform = transform.parent.parent;
        _connectorPrefab = Resources.Load<BlueprintConnector>(connectorResPath);
        _pinImage = GetComponent<Image>();
    }

    public void CreateConnector()
    {
        // var connectorGo = new GameObject("BlueprintConnector", typeof(BlueprintConnector));
        // connectorGo.AddComponent<RectTransform>();
        // var connector = connectorGo.GetComponent<BlueprintConnector>();
        // var connectorTransform = connector.transform;
        // connectorTransform.SetParent(_parentTransform);
        // connectorTransform.position = transform.position;
        var connector = Instantiate<BlueprintConnector>(
            _connectorPrefab
            , transform.position
            , Quaternion.identity
            , _parentTransform);
        connector.lineRenderer.gameObject.SetActive(false);
        connector.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        connector.transform.SetAsFirstSibling();
        if (pinType == BlueprintEntityPinType.In)
            connector.SetEntityB(entity, transform);
        else
            connector.SetEntityA(entity, transform);
        BlueprintConnector.current = connector;
        CreateConnectorOverlay();
    }

    void CreateConnectorOverlay()
    {
        var connectorGo = new GameObject("ConnectorOverlay");
        connectorGo.transform.SetParent(_parentTransform);
        connectorGo.AddComponent<RectTransform>();
        _connectorOverlay = connectorGo.GetComponent<RectTransform>();
        _connectorOverlay.sizeDelta = Vector2.zero;
        _connectorOverlay.anchorMin = new Vector2(0, 1);
        _connectorOverlay.anchorMax = new Vector2(0, 1);
        connectorGo.transform.position = Vector3.zero;
    }

    public bool DropToConnector()
    {
        var connector = BlueprintConnector.current;
        if (pinType == BlueprintEntityPinType.In)
        {
            if (connector.b != null)
            {
                Destroy(connector.gameObject);
                BlueprintConnector.current = null;
                return false;
            }
            connector.SetEntityB(entity, transform);
        }
        else
        {
            if (connector.a != null)
            {
                Destroy(connector.gameObject);
                BlueprintConnector.current = null;
                return false;
            }
            connector.SetEntityA(entity, transform);
        }
        // create block connector
        var blockConnector = new BlockConnector();
        blockConnector.a = connector.a.behaviourEntity;
        blockConnector.b = connector.b.behaviourEntity;

        if (dropToConnectorCallback != null)
        {
            dropToConnectorCallback.Invoke(blockConnector);
        }
        Debug.Log(blockConnector.a);
        Debug.Log(blockConnector.b);
        BlueprintConnector.current = null;
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CreateConnector();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_connectorOverlay != null && !_connectorOverlay.Equals(null))
        {
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _flagShownConnectorLine = 0;
        // detect an appropriate pin as oppose one
        BlueprintEntityPin detectedPin = null;
        foreach (var go in eventData.hovered)
        {
            if (go.Equals(null))
                continue;
            var pin = go.GetComponent<BlueprintEntityPin>();
            if (pin == null || pin.Equals(null))
                continue;
            if (pin.GetInstanceID() == GetInstanceID())
                continue;
            detectedPin = pin;
            break;
        }
        // if detected pin has been found
        if (detectedPin != null && !detectedPin.Equals(null))
        {
            detectedPin.DropToConnector();
        }
        else
        {
            var currentConnector = BlueprintConnector.current;
            if (currentConnector != null && !currentConnector.Equals(null))
                Destroy(currentConnector.gameObject);
        }
        Destroy(_connectorOverlay.gameObject);
        _connectorOverlay = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pinImage.color = _highlightPin;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pinImage.color = _normalPin;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pinImage.color = _highlightPin;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pinImage.color = _normalPin;
    }
}