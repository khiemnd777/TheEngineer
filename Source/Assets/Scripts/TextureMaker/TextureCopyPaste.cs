using UnityEngine;

public class TextureColorStorage
{
    static TextureColorStorage _instance;

    public static TextureColorStorage instance
    {
        get{
            return _instance ?? (_instance = new TextureColorStorage());
        }
    }

    public Color[] colors;

    public void StorageColors(Color[] colors)
    {
        this.colors = colors;
    }
}