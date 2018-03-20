using UnityEngine;
using UnityEngine.UI;

public class BlueprintEntityPositionY : BlueprintEntity
{
    public InputField number;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    public override void Start()
    {
        base.Start();
        
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