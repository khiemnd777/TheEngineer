using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMaker : MonoBehaviour 
{
	public RectTransform textureMakerRect;
	public float textureWidth;
	public float textureHeight;
	public float spaceX;
	public float spaceY;
	public TextureTile tilePrefab;
	public List<TextureTile> tiles;

	RectTransform rectTransform;

	// void Start()
	// {
	// 	var rectWidth = textureMakerRect.rect.width;
	// 	var rectHeight = textureMakerRect.rect.height;
	// 	// new Texture2D(textureWidth, textureHeight, TextureFormat.)
	// 	// pushing tiles into array by dimension of texture
	// 	tiles = new List<TextureTile>();
	// 	var numOfTile = textureWidth * textureHeight;
	// 	for(var i = 0; i < numOfTile; i++){
	// 		var tile = Instantiate<TextureTile>(tilePrefab, Vector3.zero, Quaternion.identity);
	// 		var tileRect = tile.GetComponent<RectTransform>();
	// 		tileRect.sizeDelta = new Vector2(rectWidth / textureWidth, rectHeight / textureHeight);
	// 		tiles.Add(tile);
	// 		tile.transform.SetParent(textureMakerRect);
	// 		tile.transform.localPosition = Vector3.zero;
	// 		tileRect = null;
	// 		tile = null;
	// 	}
	// 	// computing dimension of grid

	// }
}
