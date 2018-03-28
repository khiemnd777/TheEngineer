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

        input.removeConnectorCallback = () => {
            ((EntityPositionY)behaviourEntity).input = null;
        };

        output.removeConnectorCallback = () => {
            ((EntityPositionY)behaviourEntity).output = null;
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