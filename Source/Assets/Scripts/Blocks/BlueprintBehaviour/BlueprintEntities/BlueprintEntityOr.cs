using UnityEngine;

public class BlueprintEntityOr : BlueprintEntity
{
    public BlueprintEntityPin inputA;
    public BlueprintEntityPin inputB;
    public BlueprintEntityPin outputTrue;
    public BlueprintEntityPin outputFalse;

    void Start()
    {
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
    }
}