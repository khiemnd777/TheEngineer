using UnityEngine;
using UnityEngine.UI;

public class BlueprintEntityComparison : BlueprintEntity
{
    public BlueprintEntityPin inputA;
    public BlueprintEntityPin inputB;
    public BlueprintEntityPin outputTrue;
    public BlueprintEntityPin outputFalse;
    public Dropdown comparisonTypes;

    public override void Start()
    {
        base.Start();

        inputA.entity = this;
        inputB.entity = this;
        outputTrue.entity = this;
        outputFalse.entity = this;

        behaviourEntity = new EntityComparison();

        inputA.dropToConnectorCallback = blockConnector =>
        {
            ((EntityComparison)behaviourEntity).inputA = blockConnector;
        };

        inputB.dropToConnectorCallback = blockConnector =>
        {
            ((EntityComparison)behaviourEntity).inputB = blockConnector;
        };

        outputTrue.dropToConnectorCallback = blockConnector =>
        {
            ((EntityComparison)behaviourEntity).outputTrue = blockConnector;
        };

        outputFalse.dropToConnectorCallback = blockConnector =>
        {
            ((EntityComparison)behaviourEntity).outputFalse = blockConnector;
        };

        inputA.removeConnectorCallback = () =>
        {
            ((EntityComparison)behaviourEntity).inputA = null;
        };

        inputB.removeConnectorCallback = () =>
        {
            ((EntityComparison)behaviourEntity).inputB = null;
        };

        outputTrue.removeConnectorCallback = () =>
        {
            ((EntityComparison)behaviourEntity).outputTrue = null;
        };

        outputFalse.removeConnectorCallback = () =>
        {
            ((EntityComparison)behaviourEntity).outputFalse = null;
        };

        comparisonTypes.onValueChanged.AddListener((key) =>
        {
            var entity = (EntityComparison)behaviourEntity;
            entity.comparisonType = (EntityComparisonType)key;
        });
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