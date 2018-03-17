using UnityEngine;

public class BlueprintEntityPositionY : BlueprintEntity
{
    public string keyName;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    void Start()
    {
        input.entity = this;
        output.entity = this;

        behaviourEntity = new EntityPositionY();

        input.dropToConnectorCallback = blockConnector => {
            ((EntityPositionY)behaviourEntity).input = blockConnector;
        };

        output.dropToConnectorCallback = blockConnector => {
            ((EntityPositionY)behaviourEntity).output = blockConnector;
        };
    }
}