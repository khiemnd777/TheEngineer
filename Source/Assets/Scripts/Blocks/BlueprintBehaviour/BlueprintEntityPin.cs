using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintEntityPin : MonoBehaviour
{
    public BlueprintEntity entity;

    public System.Action<BlockConnector> dropToConnectorCallback;

    public void CreateConnector()
    {
        var connectorGo = new GameObject("BlueprintConnector", typeof (BlueprintConnector));
        var connector = connectorGo.GetComponent<BlueprintConnector>();
        connector.SetEntityA(entity);
        BlueprintConnector.current = connector;
    }

    public void DropToConnector()
    {
        var connector = BlueprintConnector.current;
        connector.SetEntityB(entity);

        // create block connector
        var blockConnector = new BlockConnector();
        blockConnector.a = connector.a.behaviourEntity;
        blockConnector.b = connector.b.behaviourEntity;

        if(dropToConnectorCallback != null)
        {
            dropToConnectorCallback.Invoke(blockConnector);
        }
    }
}