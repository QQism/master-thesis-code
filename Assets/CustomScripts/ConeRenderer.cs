﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HungarianAlgorithms;
public class ConeRenderer : MonoBehaviour {

	public GameObject _quad;

	[Header("Shape")]
	[Range(0, 90)]
	public float _miterAngle = 64;

	//[Range(0, 2)]
	private float _upperFaceHeight = 1.0f;

	[Range(0, 1)]
	public float _lowerFaceHeight = 1.0f;

	[Header("Appearance")]
	[Range(0, 1)]
	public float _topTransparency = 0.5f;
	[Range(0, 1)]
	public float _bottomTransparency = 0.5f;

	[Header("Tick")]
	[Range(0, 10)]
	public int _ticksCount = 4;
	[Range(0, 1)]
	public float _tickThickness = 0.005f;
	[Range(0, 1)]
	public float _tickTransparency = 1.0f;
	private Vector3 _originPosition = Vector3.zero;

	private int _quadsCount = 512;

	private float _faceHeight = 0;

	private List<GameObject> bars = new List<GameObject>();

	private int[] _barsAssignments;

	void OnValidate()
	{
		if (bars.Count == _quadsCount)
			UpdateBars();
	}

	public void initializeWithData(List<MapDataPoint> dataPoints, float maxValue)
	{
		_quadsCount = dataPoints.Count * 2;

		float rotateYAngle = 360.0f / _quadsCount;
		_faceHeight = _upperFaceHeight + _lowerFaceHeight;
        for (int i = 0; i < _quadsCount; i++)
        {
            GameObject bar = Instantiate(_quad, transform.position, Quaternion.identity);
			bar.name = "Bar " + i.ToString();
            bar.transform.SetParent(transform);
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
			traperzoid._level = 0;
			bars.Add(bar);
        }
		UpdateBars();
		mapDataPointsToBars(dataPoints, maxValue);
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
			float height = _faceHeight * Mathf.Sin(Mathf.PI/2 - miterRadAngle);

			float rotateYRadAngle = rotateYAngle * Mathf.Deg2Rad;

			float upperScale = upperBaseRadius * 2 * Mathf.Sin(rotateYRadAngle/2) / Mathf.Sin(Mathf.PI/2 - rotateYRadAngle/2);
			float lowerScale = lowerBaseRadius * 2 * Mathf.Sin(rotateYRadAngle/2) / Mathf.Sin(Mathf.PI/2 - rotateYRadAngle/2);

			GameObject bar = bars[i];
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
			traperzoid._upperScale = upperScale;
			traperzoid._lowerScale = lowerScale;
			// traperzoid._level = 0;
			traperzoid._topTransparency = _topTransparency;
			traperzoid._bottomTransparency = _bottomTransparency;
			traperzoid._ticksCount = _ticksCount;
			traperzoid._tickThickness = _tickThickness;
			traperzoid._tickTransparency = _tickTransparency;
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

	void mapDataPointsToBars(List<MapDataPoint> dataPoints, float maxValue)
	{
		var costMatrix = buildCostMatrix(dataPoints);
		_barsAssignments = HungarianAlgorithm.FindAssignments(costMatrix);

		for (int i =0; i < bars.Count; i+=2)
		{	
			var bar = bars[i];
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
			var point = dataPoints[_barsAssignments[i/2]];
			traperzoid._level = 1/maxValue * point.Value;
			traperzoid.ReCalculateScale();

			drawProjectionLine(point, bar);
		}
	}

	void drawProjectionLine(MapDataPoint point, GameObject bar)
	{
		var lineWidth = 0.05f;
		LineRenderer line = bar.AddComponent<LineRenderer>();
		var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
		line.positionCount = 2;
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		line.SetPosition(0, traperzoid.BottomBar().transform.position);		
		line.SetPosition(1, point.WorldPosition);
	}

	// Update is called once per frame
	void Update ()
	{
		for (int i =0; i < bars.Count; i+=2)
		{
			var bar = bars[i];
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
			var line = bar.GetComponent<LineRenderer>();

			if (!line)
				return;

        	line.SetPosition(0, traperzoid.BottomBar().transform.position);
		}
	}

	float[, ] buildCostMatrix(List<MapDataPoint> dataPoints)
	{
		var costs = new float[dataPoints.Count, dataPoints.Count];

		for (int i=0; i < bars.Count; i+=2)
		{
			var bar = bars[i];

			for (int j=0; j < dataPoints.Count; j++)
			{
				var point = dataPoints[j]; 
				costs[i/2, j] = Vector3.Distance(bar.transform.position, point.WorldPosition);
			}
		}
		return costs;
	}
}