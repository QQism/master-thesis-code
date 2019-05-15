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

	public float _animationSpeed = 7.0f;
	public float _textAnimationThreshold = 2.5f;

	private Vector3 _startPosition;
	// Use this for initialization
	void Awake ()
	{ 
        _rectTransform = GetComponent<RectTransform>();
		_textMesh = GetComponent<TextMeshPro>();
		_distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
		//Debug.Log("Distance: " + _distanceToCamera);
		float baseDistance = 300;
		if (_distanceToCamera > baseDistance)
		{
			_scale = (_distanceToCamera/baseDistance * 4.0f);
		}
	}
	
	void Update ()
    {
		_rectTransform.localScale = new Vector3(_scale, _scale / 8, _scale);

        switch (_selectedIdx)
        {
            case BarOption.Bar1:
				_textMesh.enabled = true;
                _textMesh.color = Color.blue;
				animate();
                break;
            case BarOption.Bar2:
				_textMesh.enabled = true;
                _textMesh.color = Color.red;
				animate();
                break;
			default:
				_textMesh.enabled = false;
				break;
        }
    }

	void animate()
	{
			Vector3 moving = new Vector3(0, Mathf.Sin(_animationSpeed * Time.time)/_textAnimationThreshold * _scale + _scale, 0);
        	_rectTransform.localPosition = new Vector3(0, 0.5f + (_scale / 8) / 2, 0);
			_rectTransform.position = _rectTransform.position + moving;
	}
}
