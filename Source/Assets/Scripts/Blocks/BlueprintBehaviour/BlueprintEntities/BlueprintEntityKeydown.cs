using UnityEngine;

public class BlueprintEntityKeydown : BlueprintEntity
{
    public string keyName;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    void Start()
    {
        input.entity = this;
        output.entity = this;

        behaviourEntity = new EntityKeydown();

        input.dropToConnectorCallback = blockConnector => {
            ((EntityKeydown)behaviourEntity).input = blockConnector;
        };

        output.dropToConnectorCallback = blockConnector => {
            ((EntityKeydown)behaviourEntity).output = blockConnector;
        };
    }
}