
public class EntityOr : BlockBehaviourEntity
{
    public BlockConnector inputA;
    public BlockConnector inputB;
    public BlockConnector outputTrue;
    public BlockConnector outputFalse;

    public override void Execute(Block block)
    {
        if(inputA == null || inputB == null)
            return;
        var consequence = System.Convert.ToBoolean(inputA.a.returnValue) 
            || System.Convert.ToBoolean(inputB.a.returnValue);
        returnValue = consequence;
        if(consequence){
            if(outputTrue == null)
                return;
            outputTrue.b.Execute(block);   
        }
        else{
            if(outputFalse == null)
                return;
            outputFalse.b.Execute(block);
        }
    }
}