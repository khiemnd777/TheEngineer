using UnityEngine;

public class EntityKeydown : BlockBehaviourEntity
{
    public string keyName;
    public BlockConnector input;
    public BlockConnector output;

    public override void Execute(Block block)
    {
        if(input == null)
            return;
        var consequence = Input.GetKeyDown(keyName);
        returnValue = consequence;
        if(consequence && output != null)
            output.b.Execute(block);
    }
}