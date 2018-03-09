using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextureTest : MonoBehaviour
{
    MeshRenderer _renderer;
    Texture2D tex;

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        tex = new Texture2D(16, 16, TextureFormat.RGBA32, true);
        for (var x = 0; x < tex.width; x++)
        {
            for (var y = 0; y < tex.height; y++)
            {
                var color = (x & y) != 0 ? Color.white : Color.grey;
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        // var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * .5f);
        // _renderer.sprite = sprite;
        var mainMaterial = _renderer.materials[0];
        mainMaterial.mainTexture = tex;
        mainMaterial.shader = Shader.Find("Sprites/Default");
        // scale 
        // transform.localScale = new Vector3(100f / tex.width, 100f / tex.height, 1);
    }

    void Update()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Color c;
        if (hit.collider != null)
        {
			c = Utility.GetTexture2DColor(tex, hit.point, transform.localPosition, transform.lossyScale);
        }
    }
}
