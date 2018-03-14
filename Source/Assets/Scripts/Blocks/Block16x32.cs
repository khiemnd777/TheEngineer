using UnityEngine;

public class Block16x32 : Block
{
    public Texture2D texture;

    public override void SetTextures(Texture2D[] textures)
    {
        texture = textures[0];

        var mainMaterial = _renderer.materials[0];
        mainMaterial.mainTexture = texture;
        mainMaterial.shader = Shader.Find("Sprites/Default");
    }
}