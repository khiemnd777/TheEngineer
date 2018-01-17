using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 vector3){
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector2 Snap2(this Vector2 vector, float snapDelta = 1f){
        var rx = Utility.Snap(vector.x, snapDelta);
        var ry = Utility.Snap(vector.y, snapDelta);
        vector.x = rx;
        vector.y = ry;
        return vector;
    }
}