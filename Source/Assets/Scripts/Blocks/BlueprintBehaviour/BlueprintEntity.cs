using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BlueprintEntity : MonoBehaviour
{
    public BlockBehaviourEntity behaviourEntity;

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void Remove()
    {
        Destroy(gameObject);
    }
}