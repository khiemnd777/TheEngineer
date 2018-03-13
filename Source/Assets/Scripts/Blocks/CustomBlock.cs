using UnityEngine;

public abstract class CustomBlock : MonoBehaviour
{
    protected System.Action<Texture2D[]> _okCallback;
    protected System.Action _cancelCallback;

    public virtual void OK()
    {
        Destroy(gameObject);
    }

    public virtual void Cancel()
    {
        Destroy(gameObject);
    }

    public virtual void SetOKCallback(System.Action<Texture2D[]> okCallback)
    {
        _okCallback = okCallback;
    }

    public virtual void SetCancelCallback(System.Action cancelCallback)
    {
        _cancelCallback = cancelCallback;
    }
}