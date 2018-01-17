using UnityEngine;

public class Utility
{
    public static float Snap(float value, float snapDelta)
    {
        // var valRound = Mathf.Abs(Mathf.Floor(value) - value) < Mathf.Abs(Mathf.Ceil(value) - value)
        //         ? Mathf.Floor(value) 
        //         : Mathf.Ceil(value);
        // return Mathf.Floor(value / snapDelta) * snapDelta + Mathf.Round((value % snapDelta) / snapDelta) * snapDelta;
        return Mathf.Floor(value / snapDelta) * snapDelta;
    }
}