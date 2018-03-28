using UnityEngine;

public class BlueprintEntityOr : BlueprintEntity
{
    public BlueprintEntityPin inputA;
    public BlueprintEntityPin inputB;
    public BlueprintEntityPin outputTrue;
    public BlueprintEntityPin outputFalse;

    public override void Start()
    {
        base.Start();
        
        inputA.entity = this;
        inputB.entity = this;
        outputTrue.entity = this;
        outputFalse.entity = this;

        behaviourEntity = new EntityOr();

        inputA.dropToConnectorCallback = blockConnector => {
            ((EntityOr)behaviourEntity).inputA = blockConnector;
        };

        inputB.dropToConnectorCallback = blockConnector => {
            ((EntityOr)behaviourEntity).inputB = blockConnector;
        };
        
        outputTrue.dropToConnectorCallback = blockConnector => {
            ((EntityOr)behaviourEntity).outputTrue = blockConnector;
        };

        outputFalse.dropToConnectorCallback = blockConnector => {
            ((EntityOr)behaviourEntity).outputFalse = blockConnector;
        };

        inputA.removeConnectorCallback = () => {
            ((EntityOr)behaviourEntity).inputA = null;
        };

        inputB.removeConnectorCallback = () => {
            ((EntityOr)behaviourEntity).inputB = null;
        };
        
        outputTrue.removeConnectorCallback = () => {
            ((EntityOr)behaviourEntity).outputTrue = null;
        };

        outputFalse.removeConnectorCallback = () => {
            ((EntityOr)behaviourEntity).outputFalse = null;
        };
    }

    public override void Remove()
    {
        if(inputA.blueprintConnector != null && !inputA.blueprintConnector.Equals(null))
            inputA.blueprintConnector.Remove();
        if(inputB.blueprintConnector != null && !inputB.blueprintConnector.Equals(null))
            inputB.blueprintConnector.Remove();
        if(outputTrue.blueprintConnector != null && !outputTrue.blueprintConnector.Equals(null))
            outputTrue.blueprintConnector.Remove();
        if(outputFalse.blueprintConnector != null && !outputFalse.blueprintConnector.Equals(null))
            outputFalse.blueprintConnector.Remove();
        base.Remove();
    }
}