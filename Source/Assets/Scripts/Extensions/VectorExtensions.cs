using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 vector3){
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector2 Round2(this Vector2 vector){
        var rx = Mathf.Abs(Mathf.Floor(vector.x) - vector.x) < Mathf.Abs(Mathf.Ceil(vector.x) - vector.x)
                ? Mathf.Floor(vector.x)
                : Mathf.Ceil(vector.x);
        var ry = Mathf.Abs(Mathf.Floor(vector.y) - vector.y) < Mathf.Abs(Mathf.Ceil(vector.y) - vector.y)
            ? Mathf.Floor(vector.y)
            : Mathf.Ceil(vector.y);
        vector.x = rx;
        vector.y = ry;
        return vector;
    }
}