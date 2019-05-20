using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoseArrowBehavior : MonoBehaviour {

	private Vector3 _startTextPosition;
	public bool _selected = false;

	public float _scale = 1;

	public float _animationSpeed = 4.0f;
	public float _textAnimationThreshold = 2.5f;
	private TextMeshPro _textMesh;

	private float _distanceToCamera;
	private MapDataPoint _mapDataPoint;

	public MapDataPoint mapDataPoint
	{
		get
		{
			return _mapDataPoint;
		}
		set
		{
			_mapDataPoint = value;
            _mapDataPoint.OnPoseEnter += OnPoseEnter;
            _mapDataPoint.OnPoseLeave += OnPoseLeave;
		}
	}
	private RectTransform _rectTransform;

	void Awake () 
	{
		_rectTransform = GetComponent<RectTransform>();	
		_startTextPosition = _rectTransform.position;
		_textMesh = GetComponent<TextMeshPro>();
		_distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
		float baseDistance = 300;
		if (_distanceToCamera > baseDistance)
		{
			_scale = (_distanceToCamera/baseDistance);
		}
	}
	
	void Update () 
	{
		
        if (_selected)
        {
			_textMesh.enabled = true;
            Vector3 moving = new Vector3(0, Mathf.Sin(_animationSpeed * Time.time) / _textAnimationThreshold + 2.2f, 0);
            _rectTransform.position = _startTextPosition + moving;
			_rectTransform.localScale = new Vector3(_scale, _scale / 8, _scale);
            _textMesh.color = Color.green;
        }
        else
        {
			_textMesh.enabled = false;
        }
	}

    void OnPoseEnter()
    {
        _selected = true;
    }

    void OnPoseLeave()
    {
        _selected = false;
    }
    void OnDestroy()
    {
        if (_mapDataPoint != null)
        {
            _mapDataPoint.OnPoseEnter -= OnPoseEnter;
            _mapDataPoint.OnPoseLeave -= OnPoseLeave;
        }
    }
}
