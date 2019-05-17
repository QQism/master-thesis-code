using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarConeArrowIndicationBehavior : MonoBehaviour {
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
	}
	
	void Update ()
    {
		//_rectTransform.localScale = new Vector3(_scale, _scale / 8, _scale);
		_rectTransform.localScale  = new Vector3(_scale/8, _scale/8, _scale/8);

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
			Vector3 moving = new Vector3(0, Mathf.Sin(_animationSpeed * Time.time)/_textAnimationThreshold * _scale/12 + _scale/20, 0);
        	_rectTransform.localPosition = new Vector3(0, 0.5f + (_scale / 8) / 2, 0);
			_rectTransform.position = _rectTransform.position + moving;
			//_rectTransform.position = _rectTransform.position;
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
}
