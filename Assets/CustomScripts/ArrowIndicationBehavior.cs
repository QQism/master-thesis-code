using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowIndicationBehavior : MonoBehaviour {
	public float _scale = 1;
	private RectTransform _rectTransform;
	private TextMeshPro _textMesh;
	private float _distanceToCamera;

	public float _animationSpeed = 7.0f;
	public float _textAnimationThreshold = 2.5f;

	private MapDataPoint.MapDataPointState _state = MapDataPoint.MapDataPointState.NotQuestion;
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
			_mapDataPoint.OnQuestionOption1ToShow += OnQuestionOption1ToShow;
			_mapDataPoint.OnQuestionOption2ToShow += OnQuestionOption2ToShow;
			_mapDataPoint.OnQuestionOption1Completed += OnQuestionOption1Completed;
			_mapDataPoint.OnQuestionOption2Completed += OnQuestionOption2Completed;
		}
	}

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

        switch (_state)
        {
            case MapDataPoint.MapDataPointState.Question1:
				_textMesh.enabled = true;
                _textMesh.color = Color.blue;
				animate();
                break;
            case MapDataPoint.MapDataPointState.Question2:
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

	void OnQuestionOption1ToShow()
	{
		_state = MapDataPoint.MapDataPointState.Question1;
	}

	void OnQuestionOption2ToShow()
	{ 
		_state = MapDataPoint.MapDataPointState.Question2;
	}

	void OnQuestionOption1Completed()
	{
		_state = MapDataPoint.MapDataPointState.NotQuestion;
	}

	void OnQuestionOption2Completed()
	{
		_state = MapDataPoint.MapDataPointState.NotQuestion;
	}
    void OnDestroy()
    {
        if (_mapDataPoint != null)
        {
			_mapDataPoint.OnQuestionOption1ToShow -= OnQuestionOption1ToShow;
			_mapDataPoint.OnQuestionOption2ToShow -= OnQuestionOption2ToShow;
			_mapDataPoint.OnQuestionOption1Completed -= OnQuestionOption1Completed;
			_mapDataPoint.OnQuestionOption2Completed -= OnQuestionOption2Completed;
        }
    }
}
