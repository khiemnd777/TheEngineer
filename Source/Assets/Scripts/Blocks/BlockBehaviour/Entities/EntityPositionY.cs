using UnityEngine;
using System.Collections;

public class EntityPositionY : BlockBehaviourEntity
{
    public float number;
    public Block block;
    public BlockConnector input;
    public BlockConnector output;

    public override void Execute()
    {
        block.StartCoroutine("Adding");
        returnValue = true;
        output.b.Execute();
    }

    IEnumerator Adding()
    {
        var percent = 0f;
        var originalPos = block.transform.position;
        var navigateToPos = new Vector3(originalPos.x, originalPos.y + number, originalPos.z);
        while(percent <= 1){
            percent += Time.deltaTime / 60;
            block.transform.position = Vector3.Lerp(originalPos, navigateToPos, percent);
            yield return null;
        }
        block.StopCoroutine("Adding");
    }
}