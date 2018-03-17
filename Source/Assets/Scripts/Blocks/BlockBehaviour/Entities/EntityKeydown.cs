using UnityEngine;

public class EntityKeydown : BlockBehaviourEntity
{
    public string keyName;
    public BlockConnector input;
    public BlockConnector output;

    public override void Execute()
    {
        returnValue = Input.GetKeyDown(keyName);
        if(returnValue)
            output.b.Execute();
    }
}