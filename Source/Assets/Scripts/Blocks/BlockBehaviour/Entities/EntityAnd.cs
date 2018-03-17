
public class EntityAnd : BlockBehaviourEntity
{
    public BlockConnector<bool> connectorAIn;
    public BlockConnector<bool> connectorBIn;
    public BlockConnector<bool> connectorTrue;
    public BlockConnector<bool> connectorFalse;

    public override void Execute()
    {
        var consequence = connectorAIn.value && connectorBIn.value;
        // connectorOut.value = consequence;
        if(consequence)
            connectorTrue.b.Execute();
        else
            connectorFalse.b.Execute();
    }
}