using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public CustomBlockUI customBlockUIPrefab;
    public Canvas canvas;

	public Block[] blocks;

    public void Create()
    {
        var customBlockUI = Instantiate(customBlockUIPrefab, Vector3.one, Quaternion.identity);
		customBlockUI.SetOKCallback((textures) => {

			customBlockUI = null;
		});
		customBlockUI.transform.SetParent(canvas.transform);
		customBlockUI.transform.position = Vector3.zero;
		customBlockUI.transform.localScale = Vector3.one;
    }
}