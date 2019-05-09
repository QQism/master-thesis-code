using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowIndicationBehavior : MonoBehaviour {
	public float _scale = 1;
	public int _selectedIdx = 1;
	private RectTransform _rectTransform;
	private TextMeshPro _textMesh;
	private float _distanceToCamera;

	// Use this for initialization
	void Awake ()
	{ 
        _rectTransform = GetComponent<RectTransform>();
		_textMesh = GetComponent<TextMeshPro>();
		_distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
		//Debug.Log("Distance: " + _distanceToCamera);
		float baseDistance = 700;
		if (_distanceToCamera > baseDistance)
		{
			_scale = (_distanceToCamera/baseDistance);
		}
	}
	
	void Update ()
    {
		_rectTransform.localScale = new Vector3(_scale, _scale / 8, _scale);
        _rectTransform.localPosition = new Vector3(0, 0.5f + (_scale / 8) / 2, 0);

        switch (_selectedIdx)
        {
            case 1:
				_textMesh.enabled = true;
                _textMesh.color = Color.red;
                break;
            case 2:
				_textMesh.enabled = true;
                _textMesh.color = Color.blue;
                break;
			default:
				_textMesh.enabled = false;
				break;
        }
    }
}
