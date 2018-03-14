using UnityEngine;

public abstract class Block : MonoBehaviour
{
    protected MeshRenderer _renderer;
    protected MeshFilter _filter;

    public abstract void SetTextures(Texture2D[] textures);

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        var quadGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _filter.mesh = quadGo.GetComponent<MeshFilter>().mesh;
        Destroy(quadGo.gameObject);
    }
}