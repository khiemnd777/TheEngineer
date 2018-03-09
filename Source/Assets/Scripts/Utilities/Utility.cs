using UnityEngine;

public class Utility
{
    public static float Snap(float value, float snapDelta)
    {
        var valApartSnap = value / snapDelta;
        var valRound = value < 0 ? Mathf.Ceil(valApartSnap) : Mathf.Floor(valApartSnap);
        return valRound * snapDelta + Mathf.Round((value % snapDelta) / snapDelta) * snapDelta;
    }

    public static Color GetTexture2DColor(Texture2D texture, Vector2 hitPoint, Vector3 localPosition, Vector3 scale)
    {
        var coord = GetTexture2DCoordinate(texture, hitPoint, localPosition, scale);
        var color = texture.GetPixel((int)coord.x, (int)coord.y);
        return color;
    }

    public static Color GetTexture2DColor(Texture2D texture, Vector2 coordinate)
    {
        var color = texture.GetPixel((int)coordinate.x, (int)coordinate.y);
        return color;
    }

    public static Vector2 GetTexture2DCoordinate(Texture2D texture, Vector2 hitPoint, Vector3 localPosition, Vector3 scale)
    {
        var hitPointX = hitPoint.x + scale.x / 2f;
        var hitPointY = hitPoint.y + scale.y / 2f;
        var x = Mathf.Abs(localPosition.x - hitPointX) / scale.x * texture.width;
        var y = Mathf.Abs(localPosition.y - hitPointY)  / scale.y * texture.height;
        return new Vector2((int)x, (int)y);
    }

    public static bool GetLocalMouseInRectTransform(GameObject go, out Vector2 result) 
    {
        var rt = ( RectTransform )go.transform;
        var mp = rt.InverseTransformPoint( Input.mousePosition );
        result.x = Mathf.Clamp( mp.x, rt.rect.min.x, rt.rect.max.x );
        result.y = Mathf.Clamp( mp.y, rt.rect.min.y, rt.rect.max.y );
        return rt.rect.Contains( mp );
    }
}