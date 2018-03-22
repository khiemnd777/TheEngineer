
public enum EntityComparisonType
{
    Equal
    , NotEqual
    , GreaterThan
    , GreaterThanAndEqual
    , LessThan
    , LessThanAndEqual
}

public class EntityComparison : BlockBehaviourEntity
{
    public EntityComparisonType comparisonType;
    public BlockConnector inputA;
    public BlockConnector inputB;
    public BlockConnector outputTrue;
    public BlockConnector outputFalse;

    public override void Execute(Block block)
    {
        if (inputA == null || inputB == null)
            return;
        var consequence = false;
        switch (comparisonType)
        {
            case EntityComparisonType.Equal:
                returnValue = inputA.a.returnValue.Equals(inputB.a.returnValue);
                break;
            case EntityComparisonType.NotEqual:
                returnValue = !inputA.a.returnValue.Equals(inputB.a.returnValue);
                break;
            case EntityComparisonType.GreaterThan:
            case EntityComparisonType.GreaterThanAndEqual:
            case EntityComparisonType.LessThan:
            case EntityComparisonType.LessThanAndEqual:
                float floatA; float floatB;
                var rawA = inputA.a.returnValue != null ? inputA.a.returnValue.ToString() : "0";
                var rawB = inputB.a.returnValue != null ? inputB.a.returnValue.ToString() : "0";
                if (float.TryParse(rawA, out floatA)
                    && float.TryParse(rawB, out floatB))
                {
                    consequence = comparisonType == EntityComparisonType.GreaterThan ? floatA > floatB
                        : comparisonType == EntityComparisonType.GreaterThanAndEqual ? floatA >= floatB
                        : comparisonType == EntityComparisonType.LessThan ? floatA < floatB
                        : comparisonType == EntityComparisonType.LessThanAndEqual ? floatA <= floatB
                        : false;
                }
                break;
        }
        returnValue = consequence;
        if (consequence)
        {
            if (outputTrue == null)
                return;
            outputTrue.b.Execute(block);
        }
        else
        {
            if (outputFalse == null)
                return;
            outputFalse.b.Execute(block);
        }
    }
}