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

    public static void AddVariable(this ExpandoObject expando, string varName, object varValue)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(varName))
            expandoDict[varName] = varValue;
        else
            expandoDict.Add(varName, varValue);
    }
}