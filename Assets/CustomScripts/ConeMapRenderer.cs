﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeMapRenderer : MonoBehaviour {

	public GameObject _quad;

	[Header("Shape")]
	public static int DEFAULT_QUADS_COUNT = 128;
	[Range(3, 1024)]
	public int _newQuadsCount = DEFAULT_QUADS_COUNT;
	private int _quadsCount = DEFAULT_QUADS_COUNT;
	[Range(0, 90)]
	public float _miterAngle = 64;

	//[Range(0, 2)]
	public float _upperFaceHeight = 1.0f;

	//[Range(0, 1)]
	public float _lowerFaceHeight = 1.0f;

	[Header("Appearance")]
	[Range(0, 1)]
	public float _transparency = 0.5f;

	public float _height;

	private Vector3 _originPosition = Vector3.zero;

	private float _faceHeight = 0;

	private List<GameObject> bars = new List<GameObject>();

	private List<MapDataPoint> _dataPoints;
	private float _maxDataPointValue;

    [SerializeField]
    public GameObject _framedBar;

    public Dictionary<MeshSelection, Mesh> _meshes;
    public MeshSelection _meshSelectionType;

	public float _barMaxValue;

	private CameraAttachedBehavior _cameraBehavior;

	void Start()
	{
		//initializeWithData(new List<MapDataPoint>(), 0);
		//mapBarToQuad(new Vector2(0.5f, 0.5f));
		_cameraBehavior = GetComponent<CameraAttachedBehavior>();
	}

	void OnValidate()
	{
		if (_newQuadsCount != _quadsCount)
		{
			_quadsCount = _newQuadsCount;
			//initializeWithData(_dataPoints, _maxDataPointValue);
		}

		if (bars.Count == _quadsCount)
			UpdateBars();

		//mapBarToQuad(new Vector2(0.5f, 0.5f));
	}

	public void initializeWithData()
	{
        _dataPoints = DataPointsManager.Instance.mapDataPoints;
        _maxDataPointValue = DataPointsManager.Instance.maxValue;

		clearData();
        for (int i = 0; i < _quadsCount; i++)
        {
            GameObject bar = Instantiate(_quad, transform.position, Quaternion.identity);
			bar.name = "Bar " + i.ToString();
            bar.transform.SetParent(transform);
			var traperzoid = bar.GetComponent<TrapezoidMapBehavior>();
			bars.Add(bar);
        }
		UpdateBars();
		calculateDataPointPositionsOnCone();
		placeDataPointsOnCone();

		if (_dataPoints.Count == 0)
			return;
	}

	public void clearData()
	{
		foreach(var bar in bars)
			Destroy(bar);

		bars.Clear();
	}

	void UpdateBars()
	{ 
		float rotateYAngle = 360.0f / _quadsCount;
		_faceHeight = _upperFaceHeight + _lowerFaceHeight;
        for (int i = 0; i < _quadsCount; i++)
        {
			float miterRadAngle = _miterAngle * Mathf.Deg2Rad;
			float upperBaseRadius = _faceHeight * Mathf.Sin(miterRadAngle);
			float lowerBaseRadius = _lowerFaceHeight * Mathf.Sin(miterRadAngle);
			_height = _faceHeight * Mathf.Sin(Mathf.PI/2 - miterRadAngle);

			float rotateYRadAngle = rotateYAngle * Mathf.Deg2Rad;

			float upperScale = upperBaseRadius * 2 * Mathf.Sin(rotateYRadAngle/2) / Mathf.Sin(Mathf.PI/2 - rotateYRadAngle/2);
			float lowerScale = lowerBaseRadius * 2 * Mathf.Sin(rotateYRadAngle/2) / Mathf.Sin(Mathf.PI/2 - rotateYRadAngle/2);

			GameObject bar = bars[i];
			var traperzoid = bar.GetComponent<TrapezoidMapBehavior>();
			traperzoid._upperScale = upperScale;
			traperzoid._lowerScale = lowerScale;
			traperzoid._transparency = _transparency;
			traperzoid._angle = rotateYAngle;
			traperzoid._miterAngle = _miterAngle;
			traperzoid._no = i;
			traperzoid._quadsCount = _quadsCount;
			traperzoid.ReCalculateScale();

			// Reset the bar rotation and position before rotating and translating
			Transform trans = bar.transform;
			trans.localPosition = _originPosition;
			trans.rotation = Quaternion.identity;
			trans.localPosition += trans.TransformDirection(trans.forward) * (upperBaseRadius + lowerBaseRadius);
			trans.RotateAround(transform.position, Vector3.up, i * rotateYAngle);
			trans.Rotate(Vector3.right * _miterAngle, Space.Self);
		}
	}

	void calculateDataPointPositionsOnCone()
	{
		Vector2 cone2DPosition = new Vector2(transform.position.x, transform.position.z);
		//Vector2 cone2DPosition = new Vector2(0, 0);
		//Vector2 cone2DPosition = new Vector2(49.1f, 38.1f);
		float maxMagnitude = 0;
		MapDataPoint furthestPoint = new MapDataPoint();
		
		foreach(MapDataPoint dataPoint in _dataPoints)
		{
			// Calculate the position of datapoint relative to the cone position
			Vector2 point2DPosition = new Vector2(dataPoint.WorldPosition.x, dataPoint.WorldPosition.z);
			//Debug.Log("Bar position (to 0xy): " + point2DPosition);
			dataPoint.ConePosition = point2DPosition - cone2DPosition;
			//Debug.Log("Bar position (to Cone): " + dataPoint.ConePosition);

			// Find the furthest datapoint
			float magnitude = dataPoint.ConePosition.magnitude;
			if (maxMagnitude < magnitude) 
			{ 
				maxMagnitude = magnitude;
				furthestPoint = dataPoint;
			}
		}

		//Debug.Log("Max magnitude: " + maxMagnitude);
		//Debug.Log("Furthest point: " + furthestPoint.Name + furthestPoint.ConePosition);
		Debug.Log("Max magnitude: " + maxMagnitude);
		scaleDataPoints(maxMagnitude);
		//scaleDataPoints(1050);
		//scaleDataPoints(1);
	}

	void scaleDataPoints(float maxMagnitude)
	{
		float buffer = 1f;
		maxMagnitude *= buffer;
		foreach(MapDataPoint dataPoint in _dataPoints)
		{
			dataPoint.ConePosition /= maxMagnitude;
			// Keep the same direction (angle) to dataPoint.ConePosition, but with the same magnitude to RawPosition, change the magnitude
			float distance = dataPoint.getDistance();

            float signedAngle = (Mathf.Atan2(dataPoint.ConePosition.y, dataPoint.ConePosition.x) - Mathf.Atan2(0, 0)) * Mathf.Rad2Deg;

            var positiveAngle = signedAngle;
            if (signedAngle < 0)
                positiveAngle = signedAngle + 4 * 90; // Convert the negative angle to all positive if it is negative

            float sinAlpha = dataPoint.ConePosition.y / dataPoint.ConePosition.x;

			float newY =  Mathf.Sin(signedAngle * Mathf.Deg2Rad) * distance;
			float newX = Mathf.Sin((90 - signedAngle) * Mathf.Deg2Rad) * distance;

			dataPoint.ConePosition = new Vector2(newX, newY);	

			if (dataPoint.Name == "Point 47")
			{
				Debug.Log("Point 47: Cone Position: " + dataPoint.ConePosition + ", Raw Position: " + dataPoint.RawPosition);
			}
			if (dataPoint.Name == "Point 48")
			{
				Debug.Log("Point 48: Cone Positiion: " + dataPoint.ConePosition + ", Raw Position: " + dataPoint.RawPosition);
			}
			//Debug.Log("Bar position (to Cone, after scale): " + dataPoint.ConePosition);
		}
	}

	void placeDataPointsOnCone()
	{
		foreach(MapDataPoint point in _dataPoints)
		{
            GameObject bar = Instantiate(_framedBar, point.WorldPosition, Quaternion.identity);
            //bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + point.Name;

            FramedConeBarData barDataComponent = bar.GetComponent<FramedConeBarData>();
			barDataComponent.mapDataPoint = point;
            barDataComponent.Value = point.Value;
            barDataComponent.LatLong = point.GeoPosition;
            barDataComponent.Elevation = 0;
            barDataComponent.MeshType = _meshSelectionType;
			//barDataComponent.MaxValue = _barMaxValue;
			barDataComponent.MaxValue = 1;//_maxDataPointValue;
			barDataComponent._static = true;

            barDataComponent.updateBars();
			barDataComponent.transform.localScale /= 100;
			mapBarToQuad(bar, point.ConePosition);
        }
	}

	// Update is called once per frame
	void Update ()
	{
	}

	void mapBarToQuad(GameObject bar, Vector2 barPosition)
	{
		//barPosition = new Vector2(_testingX, _testingY);
		Vector2 ox = new Vector2(1, 0);
		float signedAngle = (Mathf.Atan2(barPosition.y, barPosition.x) - Mathf.Atan2(ox.y, ox.x)) * Mathf.Rad2Deg;

		var positiveAngle = signedAngle;
		if (signedAngle < 0)
			positiveAngle = signedAngle + 4 * 90; // Convert the negative angle to all positive if it is negative

		//Debug.Log("Signed Angle: " + signedAngle);
		//Debug.Log("Positive Angle: " + positiveAngle);

		float rotateYAngle = 360.0f / _quadsCount;
		int quadNo = (int)((positiveAngle+rotateYAngle/2)/rotateYAngle);

		if (quadNo >= _quadsCount)
			quadNo = 0;

		int startQuad = _quadsCount/4;
		quadNo = startQuad - quadNo;
		if (quadNo < 0)
			quadNo = _quadsCount + quadNo;

		//Debug.Log("Quad: " + quadNo);

		var quad = bars[quadNo];
		var traperzoid = quad.GetComponent<TrapezoidMapBehavior>();

		// Determine the angle relative to the quad
		var quadAngle = (startQuad - quadNo) * rotateYAngle;
		//Debug.Log("quadAngle: " + quadAngle);

		var convertedSignedAngle = signedAngle + (90 - quadAngle);
		//Debug.Log("Bar magnitude: " + barPosition.magnitude);
		traperzoid.addObjectOnSurface(bar, convertedSignedAngle, barPosition.magnitude);
	}

	public void controlerUpdate(ControllerBehavior controller) 
	{
		bool changed = false;
		if (controller.isIncreasingAngle())
		{
			Debug.Log("Increase Angle");
			if (_miterAngle < 90)
			{
				_miterAngle += 1;
				changed = true;
			} else
			{ 
				controller.triggerHapticPulse(2);
			}
		}

		if (controller.isDecreasingAngle())
		{
			Debug.Log("Decrease Angle");
			if (_miterAngle > 0)
			{
				_miterAngle -= 1;
				changed = true;
			} else
			{
				controller.triggerHapticPulse(2);
			}
		}

		//if (controller.isIncreasingHeight())
		//{
		//	Debug.Log("Increase Height");
		//	_cameraBehavior._coneHeight += 0.01f;
		//}

		//if (controller.isDecreasingHeight())
		//{
		//	Debug.Log("Decrease Height");
		//	_cameraBehavior._coneHeight -= 0.01f;
		//}

		if (controller.isIncreasingInnerCirle())
		{
			Debug.Log("Increase Inner Circle");
            if (_lowerFaceHeight < 1)
			{
                _lowerFaceHeight += 0.02f;
                changed = true;
			}
            else {
				controller.triggerHapticPulse(2);
			}
		}

		if (controller.isDecreasingInnerCircle())
		{
			Debug.Log("Decrease Inner Circle");
            if (_lowerFaceHeight > 0)
            {
                _lowerFaceHeight -= 0.02f;
                changed = true;
            } else {
				controller.triggerHapticPulse(2);
			}
		}

		if (changed)
			OnValidate();
	}
}