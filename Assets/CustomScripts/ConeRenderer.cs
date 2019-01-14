using System.Collections;
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

	private int _quadsCount = 1024;

	private float _faceHeight = 0;

	private List<GameObject> bars = new List<GameObject>();

	private int[] _barsAssignments;

	private float _projectionLineWidth = 0.05f;

	private List<MapDataPoint> _dataPoints;

	void OnValidate()
	{
		if (bars.Count == _quadsCount)
			UpdateBars();
	}

	public void initializeWithData(List<MapDataPoint> dataPoints, float maxValue)
	{
		_dataPoints = dataPoints;

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
		mapDataPointsToBars(maxValue);
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

	void mapDataPointsToBars(float maxValue)
	{
		var costMatrix = buildCostMatrix(_dataPoints);
		_barsAssignments = HungarianAlgorithm.FindAssignments(costMatrix);

		for (int i=0; i < _dataPoints.Count; i++)
		{	
			var point = _dataPoints[i];
			var bar = bars[_barsAssignments[i]*2];
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();

			traperzoid._level = 1/maxValue * point.Value;
			traperzoid.ReCalculateScale();

			drawProjectionLine(point, bar);
		}
	}

	void drawProjectionLine(MapDataPoint point, GameObject bar)
	{
		var lineWidth = 0.05f;
		LineRenderer line = bar.GetComponentInChildren<LineRenderer>();
		line.enabled = true;
		var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
		line.positionCount = 2;
		lineWidth = 0.005514949f * 2;
		line.startWidth = lineWidth;
		//line.endWidth = lineWidth;
		line.endWidth = lineWidth * Vector3.Distance(traperzoid.BottomBar().transform.position, point.WorldPosition);
		line.SetPosition(0, traperzoid.BottomBar().transform.position);		
		line.SetPosition(1, point.WorldPosition);
	}

	void drawProjectionHead()
	{

	}

	void drawProjectionTail()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		for (int i=0; i < bars.Count; i+=2)
		{
			var bar = bars[i];
			var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
			var line = bar.GetComponentInChildren<LineRenderer>();

			if (!line.enabled)
				continue;

			var bottomBar = traperzoid.BottomBar();
			var topZ = bottomBar.transform.localPosition.z + bottomBar.transform.localScale.z;
			var lineStartPosition = bottomBar.transform.TransformPoint(new Vector3(0, 0, 1));
        	//line.SetPosition(0, bottomBar.transform.position);
        	line.SetPosition(0, lineStartPosition);
		}
	}

	float[, ] buildCostMatrix(List<MapDataPoint> dataPoints)
	{
		var costs = new float[dataPoints.Count, _quadsCount/2];

		for (int i=0; i < dataPoints.Count; i++)
		{
			var point = dataPoints[i]; 

			for (int j=0; j < _quadsCount; j+=2)
			{
				var bar = bars[j]; 
				var traperzoid = bar.GetComponent<TrapezoidBarBehavior>();
				// costs[i, j/2] = Vector2.Distance(
				// 		new Vector2(traperzoid.BottomBar().transform.position.x, traperzoid.BottomBar().transform.position.z),
				// 		new Vector2(point.WorldPosition.x, point.WorldPosition.z));
				costs[i, j/2] = Vector3.Distance(traperzoid.BottomBar().transform.position, point.WorldPosition);
			}
		}
		return costs;
	}
}