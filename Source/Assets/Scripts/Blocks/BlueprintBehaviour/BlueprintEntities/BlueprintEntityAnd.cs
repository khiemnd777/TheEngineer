using UnityEngine;

public class BlueprintEntityAnd : BlueprintEntity
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

        behaviourEntity = new EntityAnd();

        inputA.dropToConnectorCallback = blockConnector =>
        {
            ((EntityAnd)behaviourEntity).inputA = blockConnector;
        };

        inputB.dropToConnectorCallback = blockConnector =>
        {
            ((EntityAnd)behaviourEntity).inputB = blockConnector;
        };

        outputTrue.dropToConnectorCallback = blockConnector =>
        {
            ((EntityAnd)behaviourEntity).outputTrue = blockConnector;
        };

        outputFalse.dropToConnectorCallback = blockConnector =>
        {
            ((EntityAnd)behaviourEntity).outputFalse = blockConnector;
        };

        inputA.removeConnectorCallback = () =>
        {
            ((EntityAnd)behaviourEntity).inputA = null;
        };

        inputB.removeConnectorCallback = () =>
        {
            ((EntityAnd)behaviourEntity).inputB = null;
        };

        outputTrue.removeConnectorCallback = () =>
        {
            ((EntityAnd)behaviourEntity).outputTrue = null;
        };

        outputFalse.removeConnectorCallback = () =>
        {
            ((EntityAnd)behaviourEntity).outputFalse = null;
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