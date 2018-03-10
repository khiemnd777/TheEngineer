using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMakerAction : MonoBehaviour
{
    public RectTransform textureEditorPrefab;

	Canvas _canvas;
    RectTransform _textureEditor;
    TextureMaker _maker;
    bool _isMoveInside;

    void Start()
    {
        _maker = GetComponent<TextureMaker>();
		_canvas = GetComponentInParent<Canvas>();

		_maker.readOnly = true;
    }

    void Update()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            _isMoveInside = true;
        }
        else
        {
            _isMoveInside = false;
        }
    }

    void OnMouseDown()
    {
        if (_isMoveInside)
        {
			
        }
    }

    void OnMouseUp()
    {
        if (_isMoveInside)
        {
			InstanceTextureEditor();
        }
    }

    void InstanceTextureEditor()
    {
        _textureEditor = Instantiate(textureEditorPrefab, Vector3.one, Quaternion.identity);
		var maker = _textureEditor.GetComponentInChildren<TextureMaker>();
		maker.SetLookGoodCallback(colors => {
			_maker.SetColors(colors);
			Destroy(_textureEditor.gameObject);
			_textureEditor = null;
		});
		maker.SetColors(_maker.colors);
		_textureEditor.transform.SetParent(_canvas.transform);
		_textureEditor.transform.position = Vector3.one;
		_textureEditor.transform.localScale = Vector3.one;
    }
}