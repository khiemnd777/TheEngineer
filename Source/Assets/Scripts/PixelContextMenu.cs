using UnityEngine;
using UnityEngine.UI;

public class PixelContextMenu : ContextMenu
{
    public Button scriptItPrefab;
    public Button takeOffPrefab;

    public override void Register()
    {
        RegisterItem("script-it", "Script It", scriptItPrefab, () =>
        {

        });

        RegisterItem("take-off", "Take Off", takeOffPrefab, () =>
        {

        });
    }
}