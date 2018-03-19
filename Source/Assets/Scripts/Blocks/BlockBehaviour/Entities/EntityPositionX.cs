using UnityEngine;
using System.Collections;

public class EntityPositionX : BlockBehaviourEntity
{
    public float number;
    public BlockConnector input;
    public BlockConnector output;

    Block _block;

    public override void Execute(Block block)
    {
        _block = block;
        _block.StartCoroutine("Adding");
        returnValue = true;
        if(output != null)
            output.b.Execute(block);
    }

    IEnumerator Adding()
    {
        var percent = 0f;
        var originalPos = _block.transform.position;
        var navigateToPos = new Vector3(originalPos.x + number, originalPos.y, originalPos.z);
        while(percent <= 1){
            percent += Time.deltaTime / 60;
            _block.transform.position = Vector3.Lerp(originalPos, navigateToPos, percent);
            yield return null;
        }
        _block.StopCoroutine("Adding");
    }
}