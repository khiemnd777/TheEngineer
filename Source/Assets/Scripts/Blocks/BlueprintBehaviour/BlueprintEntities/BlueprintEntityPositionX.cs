using UnityEngine;

public class BlueprintEntityPositionX : BlueprintEntity
{
    public string keyName;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    void Start()
    {
        input.entity = this;
        output.entity = this;

        behaviourEntity = new EntityPositionX();

        input.dropToConnectorCallback = blockConnector => {
            ((EntityPositionX)behaviourEntity).input = blockConnector;
        };

        output.dropToConnectorCallback = blockConnector => {
            ((EntityPositionX)behaviourEntity).output = blockConnector;
        };
    }
}