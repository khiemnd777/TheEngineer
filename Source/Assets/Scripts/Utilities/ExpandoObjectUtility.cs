using UnityEngine;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

public class ExpandoObjectUtility
{
    public static void SetVariable(ExpandoObject expando, string varName, object varValue)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(varName))
            expandoDict[varName] = varValue;
        else
            expandoDict.Add(varName, varValue);
    }

    public static object GetVariable(ExpandoObject expando, string varName)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (!expandoDict.ContainsKey(varName))
            return null;
        return expandoDict[varName];
    }
}