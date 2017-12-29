using UnityEngine;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public static class ObjectExtensions
{
    public static bool IsNull(this object target)
    {
        return target == null || target is Object && target.Equals(null);
    }
}