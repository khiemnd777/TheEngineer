using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour
{
    public Block block;

    public BlockConnector[] anyState;

    public void Init()
    {
        anyState = new BlockConnector[5];
    }

    public void Execute()
    {
        foreach(var state in anyState)
        {
            state.b.Execute();
        }
    }
}