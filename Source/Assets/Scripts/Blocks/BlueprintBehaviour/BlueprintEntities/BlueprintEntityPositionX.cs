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

        input.removeConnectorCallback = () => {
            ((EntityPositionX)behaviourEntity).input = null;
        };

        output.removeConnectorCallback = () => {
            ((EntityPositionX)behaviourEntity).output = null;
        };
    }

    public override void Remove()
    {
        if(input.blueprintConnector != null && !input.blueprintConnector.Equals(null))
            input.blueprintConnector.Remove();
        if(output.blueprintConnector != null && !output.blueprintConnector.Equals(null))
            output.blueprintConnector.Remove();
        base.Remove();
    }
}