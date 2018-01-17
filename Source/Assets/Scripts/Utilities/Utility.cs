using UnityEngine;

public class Utility
{
    public static float Snap(float value, float snapDelta)
    {
        var valApartSnap = value / snapDelta;
        var valRound = value < 0 ? Mathf.Ceil(valApartSnap) : Mathf.Floor(valApartSnap);
        return valRound * snapDelta + Mathf.Round((value % snapDelta) / snapDelta) * snapDelta;
    }
}