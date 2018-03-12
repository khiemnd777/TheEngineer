using UnityEngine;

public class CustomBlockUI : MonoBehaviour
{
    public TextureMakerAction surfaceTexture;
    public TextureMakerAction standardTexture;

    System.Action<Texture2D[]> _okCallback;

    public void OK()
    {
        if(_okCallback != null)
            _okCallback.Invoke(new [] { surfaceTexture.maker.GetTexture(), standardTexture.maker.GetTexture() });
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }

    public void SetOKCallback(System.Action<Texture2D[]> okCallback)
    {
        _okCallback = okCallback;
    }
}