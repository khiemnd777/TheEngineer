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
    }
}