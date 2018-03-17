using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockBehaviour behaviour;

    void Update()
    {
        if(behaviour != null)
            behaviour.Execute();
    }

    public void SetBehaviour(BlockBehaviour behaviour)
    {
        behaviour.block = this;
        this.behaviour = behaviour;
    }
}