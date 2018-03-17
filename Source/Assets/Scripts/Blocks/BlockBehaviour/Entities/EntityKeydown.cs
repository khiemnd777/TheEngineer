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
        returnValue = Input.GetKeyDown(keyName);
        if(returnValue && output != null)
            output.b.Execute(block);
    }
}