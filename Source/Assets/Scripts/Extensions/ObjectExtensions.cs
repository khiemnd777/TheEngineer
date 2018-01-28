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

    public static bool IsNotNull(this object target)
    {
        return !IsNull(target);
    }

    public static int GetID(this Component target)
    {
        return target.transform.GetInstanceID();
    }

    public static bool HasRectTransform(this Component target)
    {
        return target.GetComponent<RectTransform>().IsNotNull();
    }

    public static bool IsAssignedFor(this object target, System.Type assignedForType)
    {
        return assignedForType.IsAssignableFrom(target.GetType());
    }

    public static bool IsAssignedFor<T>(this object target)
    {
        return IsAssignedFor(target, typeof(T));
    }
}