using UnityEngine;

public static class ObjectExtensions
{
    public static bool IsNull(this object target)
    {
        return target == null || target is Object && target.Equals(null);
    }
}