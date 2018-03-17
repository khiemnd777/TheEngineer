
public class EntityAnd : BlockBehaviourEntity
{
    public BlockConnector inputA;
    public BlockConnector inputB;
    public BlockConnector outputTrue;
    public BlockConnector outputFalse;

    public override void Execute()
    {
        var consequence = inputA.a.returnValue && inputB.a.returnValue;
        returnValue = consequence;
        if(consequence)
            outputTrue.b.Execute();
        else
            outputFalse.b.Execute();
    }
}