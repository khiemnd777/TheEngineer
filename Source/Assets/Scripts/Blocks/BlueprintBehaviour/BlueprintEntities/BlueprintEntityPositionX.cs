using UnityEngine;
using UnityEngine.UI;

public class BlueprintEntityPositionX : BlueprintEntity
{
    public InputField number;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    public override void Start()
    {
        base.Start();
        
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