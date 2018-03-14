using UnityEngine;

public class Block16x16 : Block
{
    public Texture2D surfaceTexture;
    public Texture2D standardTexture;

    public override void SetTextures(Texture2D[] textures)
    {
        surfaceTexture = textures[0];
        standardTexture = textures[1];

        var mainMaterial = _renderer.materials[0];
        mainMaterial.mainTexture = standardTexture;
        mainMaterial.shader = Shader.Find("Sprites/Default");
    }
}