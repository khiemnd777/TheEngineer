using UnityEngine;

public class PixelRemovingManager : MonoBehaviour
{
    void Update()
    {
        if (
            // macOS
            ((Input.GetKey(KeyCode.LeftCommand)
                || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyUp(KeyCode.Backspace))
            // windowsOS
            || Input.GetKeyUp(KeyCode.Delete)
            )
        {
            var pixels = FindObjectsOfType<Pixel>();
            foreach (var pixel in pixels)
            {
                if (!pixel.selecting)
                    continue;
                Destroy(pixel.gameObject);
            }
        }
    }
}