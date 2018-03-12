using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D), typeof(MeshFilter))]
public class TextureMaker : MonoBehaviour
{
	public int textureWidth;
	public int textureHeight;
	public bool readOnly;
	public Color paintColor;
	public CUIColorPicker colorPicker;
	public Color[] colors{
		get{
			return _colors;
		}
	}
	Color[] _colors;
	Color _hoveredColor;
	Vector2 _hoveredCoordinate;
	Texture2D _texture;
	MeshRenderer _renderer;
	MeshFilter _filter;
	System.Action<Color[]> _lookGoodCallback;
	TextureColorStorage _textureColorStorage;
	bool _isMoveInside;

	void Awake()
	{
		readOnly = false;
		_textureColorStorage = TextureColorStorage.instance;
		_renderer = GetComponent<MeshRenderer>();
		_filter = GetComponent<MeshFilter>();

		// register colorPicker value change event
		if(colorPicker != null)
			colorPicker.SetOnValueChangeCallback(c => {
				paintColor = c;
			});

		// init mesh filter of Quad
		var quadGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
		_filter.mesh = quadGo.GetComponent<MeshFilter>().mesh;
		Destroy(quadGo.gameObject);
		// init paint color
		var initColor = Color.white;
		paintColor = colorPicker == null ? Color.white : colorPicker.Color;
		paintColor.a = 1;
		// create instance of texture
		_texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, true);
		_colors = new Color[textureWidth * textureHeight];
		for(var x = 0; x < _texture.width; x++){
			for(var y = 0; y < _texture.height; y++){
				//var initColor = Color.white; //(x & y) != 0 ? Color.white : Color.grey;
				_colors[x + y  * _texture.width] = initColor;
			}
		}
		_texture.SetPixels(_colors);
		_texture.Apply();
		_texture.wrapMode = TextureWrapMode.Clamp;
        _texture.filterMode = FilterMode.Point;
		// assign into material renderer's texture
		var mainMaterial = _renderer.materials[0];
        mainMaterial.mainTexture = _texture;
        mainMaterial.shader = Shader.Find("Sprites/Default");

		// start drag event;
		StartCoroutine(Dragging());
	}

	void Update()
    {
		// hit on texture's coordinates;
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
			_isMoveInside = true;
			_hoveredCoordinate = Utility.GetTexture2DCoordinate(_texture, hit.point, transform.position, transform.lossyScale);
			_hoveredColor = Utility.GetTexture2DColor(_texture, _hoveredCoordinate);
        }else{
			_isMoveInside = false;
		}
    }

	void DragStart()
	{
		if(_isMoveInside)
		{
			if(colorPicker && colorPicker.usePicker){
				var x = (int)_hoveredCoordinate.x;
				var y = (int)_hoveredCoordinate.y;
				var p = x + y * _texture.width;
				if(p < _texture.width * _texture.height){
					colorPicker.Color = _texture.GetPixel(x, y);
				}
				colorPicker.UsePickerOff();
			}
		}
	}

	void Drag()
	{
		try{
			if(readOnly)
				return;
			var x = (int)_hoveredCoordinate.x;
			var y = (int)_hoveredCoordinate.y;
			var p = x + y * _texture.width;
			if(p >= _texture.width * _texture.height)
				return;
			_colors[p] = paintColor;
			_texture.SetPixels(_colors);
			_texture.Apply();
		}catch{
			throw;
		}
	}

	void Drop()
	{
		
	}

	IEnumerator Dragging()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
				if(_isMoveInside)
					Drag();
            }
            yield return null;
        }
    }

	void OnMouseDown()
    {
        DragStart();
    }

    void OnMouseUp()
    {
        Drop();
    }

	void FillOver()
	{
		for(var x = 0; x < _texture.width; x++){
			for(var y = 0; y < _texture.height; y++){
				_colors[x + y  * _texture.width] = paintColor;
			}
		}
		_texture.SetPixels(_colors);
		_texture.Apply();
	}

	public void LookGood()
	{
		if(_lookGoodCallback != null)
			_lookGoodCallback.Invoke(_colors);
	}

	public void Copy()
	{
		_textureColorStorage.StorageColors(_colors);
	}

	public void Paste()
	{
		SetColors(_textureColorStorage.colors);
	}

	public void SetColors(Color[] colors)
	{
		for(var x = 0; x < _texture.width; x++){
			for(var y = 0; y < _texture.height; y++){
				var position = x + y  * _texture.width;
				_colors[position] = position < colors.Length ? colors[position] : Color.white;
			}
		}
		_texture.SetPixels(_colors);
		_texture.Apply();
	}

	public Texture2D GetTexture()
	{
		return _texture;
	}

	public void Fill()
	{	
		FillOver();
	}
	
	public void SetLookGoodCallback(System.Action<Color[]> callback)
    {
        _lookGoodCallback = callback;
    }
}
