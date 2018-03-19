using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour
{
    public Block block;

    public BlockConnector[] anyState;

    public BlockBehaviour()
    {
        Init();
    }

    public void Init()
    {
        anyState = new BlockConnector[5];
    }

    // This function will be run in the Update function.
    // It updates per frame.
    public void Execute()
    {
        foreach(var state in anyState)
        {
            state.b.Execute(block);
        }
    }
}