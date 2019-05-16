using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezoidBarBehavior : MonoBehaviour {

	[Header("Shape Transformations")]
	[Range(0, 5)]
	public float _upperScale = 1.0f;
	[Range(0, 5)]
	public float _lowerScale = 1.0f;

	[Header("Data Volume")]
	[Range(0, 1)]
	public float _level = 0.5f;
	private GameObject _frameBar;
	private GameObject _dataBar;
    [SerializeField]
    public GameObject _indicationArrow;

	[Header("Main Colors")]
	[Range(0, 1)]
	public float _topTransparency;
	[Range(0, 1)]
	public float _bottomTransparency;

	[Header("Ticks Customization")]
	[Range(0, 10)]
	public int _ticksCount = 0;

	[Range(0, 1)]
	public float _tickThickness = 0.01f;

	[Range(0, 1)]
	public float _tickTransparency = 0.8f;

	public float _angle = 0;

	public float _no = 0;

	public float _miterAngle = 90;

	public int _quadsCount = 0;

	private Material dataMaterial;
	private Material frameMaterial;

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
            var arrow = _indicationArrow.GetComponent<SmallArrowIndicationBehavior>();
            arrow.mapDataPoint = value;
            _mapDataPoint.OnPoseEnter += onPoseEnter;
            _mapDataPoint.OnPoseLeave += onPoseLeave;

            PoseBehavior dataPose = _dataBar.gameObject.AddComponent(typeof(PoseBehavior)) as PoseBehavior;
            if (dataPose)
            {
                dataPose.onPoseEnter += _mapDataPoint.poseEnter;
                dataPose.onPoseLeave += _mapDataPoint.poseLeave;
            }
            PoseBehavior framePose = _frameBar.gameObject.AddComponent(typeof(PoseBehavior)) as PoseBehavior;
            if (framePose)
            {
                framePose.onPoseEnter += _mapDataPoint.poseEnter;
                framePose.onPoseLeave += _mapDataPoint.poseLeave;
            }
		}
	}

	void Awake()
	{
		GameObject container = transform.Find("RotationContainer").gameObject;
		_frameBar = container.transform.Find("Top").gameObject;
		_dataBar = container.transform.Find("Bottom").gameObject;

		// Create copies of materials 
		Renderer topRenderer =  _frameBar.GetComponent<Renderer>();
		Renderer bottomRenderer =  _dataBar.GetComponent<Renderer>();

        frameMaterial = new Material(topRenderer.sharedMaterial);
        dataMaterial = new Material(bottomRenderer.sharedMaterial);

		topRenderer.sharedMaterial = frameMaterial;
		bottomRenderer.sharedMaterial = dataMaterial;
	}

	void OnValidate()
	{
		ReCalculateScale();
	}

	public GameObject BottomBar()
	{
		return _dataBar;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void ReCalculateScale()
	{
		if (_frameBar == null || _dataBar == null)
			return;

		float bottomScale = _level;
		float topScale = 1 - _level;

		_dataBar.transform.localScale = new Vector3(1, 1, bottomScale);
		_frameBar.transform.localScale = new Vector3(1, 1, topScale);

		Vector3 oldBottomPosition = _dataBar.transform.localPosition;
		Vector3 oldTopPosition = _frameBar.transform.localPosition;

		_dataBar.transform.localPosition = new Vector3(oldBottomPosition.x, oldBottomPosition.y, _level - 1);
		_frameBar.transform.localPosition = new Vector3(oldTopPosition.x, oldTopPosition.y, _level);

		Material topMaterial =  _frameBar.GetComponent<Renderer>().sharedMaterial;
		Material bottomMaterial =  _dataBar.GetComponent<Renderer>().sharedMaterial;

		if (topMaterial == null || bottomMaterial == null)
			return;

		float midScale = _upperScale + (_lowerScale -_upperScale) * (1 - _level);
		topMaterial.SetFloat("_UpperScale", _upperScale);
		topMaterial.SetFloat("_LowerScale", midScale);

		bottomMaterial.SetFloat("_UpperScale", midScale);
		bottomMaterial.SetFloat("_LowerScale", _lowerScale);

		topMaterial.SetInt("_OnTop", 1);
		bottomMaterial.SetInt("_OnTop", 0);

		topMaterial.SetFloat("_LevelScale", topScale);
		bottomMaterial.SetFloat("_LevelScale", bottomScale);

		float tickStep = 1.0f / (_ticksCount + 1);
		topMaterial.SetFloat("_TickStep", tickStep);
		bottomMaterial.SetFloat("_TickStep", tickStep);

		topMaterial.SetFloat("_TickThickness", _tickThickness / topScale);
		bottomMaterial.SetFloat("_TickThickness", _tickThickness  / bottomScale);

		topMaterial.SetFloat("_Transparency", _topTransparency);
		bottomMaterial.SetFloat("_Transparency", _bottomTransparency);

		topMaterial.SetFloat("_TickTransparency", _tickTransparency);
		bottomMaterial.SetFloat("_TickTransparency", _tickTransparency);

		topMaterial.SetFloat("_RotationAngle", _angle * _no);
		topMaterial.SetFloat("_MiterAngle", _miterAngle);

		topMaterial.SetInt("_QuadsCount", _quadsCount);
	}

    void onPoseEnter()
    {
        Debug.Log("Pose enter: " + name);
        dataMaterial.SetInt("_OutlineOn", 1);
        frameMaterial.SetInt("_OutlineOn", 1);
        mapDataPoint.Selected = true;
    }

    void onPoseLeave()
    {
        Debug.Log("Pose leave: " + name);
        dataMaterial.SetInt("_OutlineOn", 0);
        frameMaterial.SetInt("_OutlineOn", 0);
        mapDataPoint.Selected = false;
    }
}
