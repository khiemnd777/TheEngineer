using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public class BlueprintEntityContextMenuRegistrar : ContextMenuRegistrar
{
    BlueprintEntity _entity;

    protected override void Start()
    {
        base.Start();
        _entity = GetComponent<BlueprintEntity>();
    }

    public override void Register()
    {
        RegisterItem("delete", "Delete", () =>{
            _entity.Remove();
        });
    }
}