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

        comparisonTypes.onValueChanged.AddListener((key) =>
        {
            var entity = (EntityComparison)behaviourEntity;
            entity.comparisonType = (EntityComparisonType)key;
        });
    }
}