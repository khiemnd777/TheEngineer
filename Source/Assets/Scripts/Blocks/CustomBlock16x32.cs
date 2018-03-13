using UnityEngine;

public class CustomBlock16x32 : CustomBlock
{
    public TextureMakerAction texture;

    public override void OK()
    {
        if(_okCallback != null)
            _okCallback.Invoke(new [] { texture.maker.GetTexture() });
        base.OK();
    }
}