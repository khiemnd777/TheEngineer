using UnityEngine;

public class Blueprint : MonoBehaviour
{
    public Block block;
    
    void Start()
    {
        var behaviour = new BlockBehaviour();
        block.SetBehaviour(behaviour);
    }
}