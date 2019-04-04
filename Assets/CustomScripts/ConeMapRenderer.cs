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

	public Vector2 _testingDotPosition;
	public GameObject _testingDot;

	void Start()
	{
		initializeWithData(new List<MapDataPoint>(), 0);
		mapBarToQuad(new Vector2(0.5f, 0.5f));
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
	}

	public void initializeWithData(List<MapDataPoint> dataPoints, float maxValue)
	{
		_dataPoints = dataPoints;
		_maxDataPointValue = maxValue;

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

		if (_dataPoints.Count == 0)
			return;
	}

	private void clearData()
	{
		foreach(var bar in bars)
			DestroyImmediate(bar);

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

	// Update is called once per frame
	void Update ()
	{
	}

	void mapBarToQuad(Vector2 barPosition)
	{
		barPosition = new Vector2(.5f, 0f);
		Vector2 ox = new Vector2(1, 0);
		float signedAngle = (Mathf.Atan2(barPosition.y, barPosition.x) - Mathf.Atan2(ox.y, ox.x)) * Mathf.Rad2Deg;

		Debug.Log(signedAngle);

		if (signedAngle < 0)
			signedAngle = signedAngle + 4 * 90; // Convert the negative angle to all positive if it is negative

		Debug.Log(signedAngle);

		float rotateYAngle = 360.0f / _quadsCount;
		int quadNo = (int)((signedAngle+rotateYAngle/2)/rotateYAngle);

		if (quadNo >= _quadsCount)
			quadNo = 0;

		Debug.Log(quadNo);

		//TODO: Given the angle & the distance, calculate the position of the bar on top of the quad
		GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		dot.GetComponent<Renderer>().material = _testingDot.GetComponent<Renderer>().sharedMaterial;
		var quad = bars[quadNo];
		var traperzoid = quad.GetComponent<TrapezoidMapBehavior>();
		traperzoid.addObjectOnSurface(dot, 0, 0);

	}
}