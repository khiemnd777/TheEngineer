using UnityEngine;

public class CustomBlock16x16 : CustomBlock
{
    public TextureMakerAction surfaceTexture;
    public TextureMakerAction standardTexture;

    public override void OK()
    {
        if(_okCallback != null)
            _okCallback.Invoke(new [] { surfaceTexture.maker.GetTexture(), standardTexture.maker.GetTexture() });
        base.OK();
    }
}