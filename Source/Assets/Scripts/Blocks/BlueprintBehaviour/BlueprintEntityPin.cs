using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BlueprintEntityPinType
{
    In, Out
}

public class BlueprintEntityPin : MonoBehaviour
    , IBeginDragHandler
    , IDragHandler
    , IEndDragHandler
{
    [System.NonSerialized]
    public BlueprintEntity entity;
    public BlueprintEntityPinType pinType;

    public System.Action<BlockConnector> dropToConnectorCallback;

    public void CreateConnector()
    {
        var connectorGo = new GameObject("BlueprintConnector", typeof(BlueprintConnector));
        var connector = connectorGo.GetComponent<BlueprintConnector>();
        if (pinType == BlueprintEntityPinType.In)
            connector.SetEntityB(entity);
        else
            connector.SetEntityA(entity);
        BlueprintConnector.current = connector;
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
            connector.SetEntityB(entity);
        }
        else
        {
            if (connector.a != null)
            {
                Destroy(connector.gameObject);
                BlueprintConnector.current = null;
                return false;
            }
            connector.SetEntityA(entity);
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

    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        // DropToConnector();
    }
}