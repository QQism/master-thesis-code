using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezoidBarBehavior : MonoBehaviour {

	[Range(0, 5)]
	public float _upperScale = 1.0f;
	[Range(0, 5)]
	public float _lowerScale = 1.0f;

	[Range(0, 1)]
	public float _level = 0.5f;
	private GameObject _topBar;
	private GameObject _bottomBar;

	[Range(0, 1)]
	public float _topTranparency;
	[Range(0, 1)]
	public float _bottomTranparency;

	[Range(0, 10)]
	public int _ticksCount = 0;

	[Range(0, 1)]
	public float _tickThickness = 0.01f;
	// Use this for initialization

	void Start () {
	}

	void Awake()
	{
		GameObject container = transform.Find("RotationContainer").gameObject;
		_topBar = container.transform.Find("Top").gameObject;
		_bottomBar = container.transform.Find("Bottom").gameObject;

		// Create copies of materials 
		Renderer topRenderer =  _topBar.GetComponent<Renderer>();
		Renderer bottomRenderer =  _bottomBar.GetComponent<Renderer>();

        Material topMaterial = new Material(topRenderer.sharedMaterial);
        Material bottomMaterial = new Material(bottomRenderer.sharedMaterial);

		topRenderer.sharedMaterial = topMaterial;
		bottomRenderer.sharedMaterial = bottomMaterial;
	}

	void OnValidate()
	{
		ReCalculateScale();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void ReCalculateScale()
	{
		if (_topBar == null || _bottomBar == null)
			return;

		float bottomScale = _level;
		float topScale = 1 - _level;

		_bottomBar.transform.localScale = new Vector3(1, 1, bottomScale);
		_topBar.transform.localScale = new Vector3(1, 1, topScale);

		Vector3 oldBottomPosition = _bottomBar.transform.localPosition;
		Vector3 oldTopPosition = _topBar.transform.localPosition;

		_bottomBar.transform.localPosition = new Vector3(oldBottomPosition.x, oldBottomPosition.y, _level - 1);
		_topBar.transform.localPosition = new Vector3(oldTopPosition.x, oldTopPosition.y, _level);

		Material topMaterial =  _topBar.GetComponent<Renderer>().sharedMaterial;
		Material bottomMaterial =  _bottomBar.GetComponent<Renderer>().sharedMaterial;

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
		Debug.Log("Stick step: " + tickStep);
		topMaterial.SetFloat("_TickStep", tickStep);
		bottomMaterial.SetFloat("_TickStep", tickStep);

		// topMaterial.SetFloat("_TickThickness", _tickThickness * (topScale * (1 - bottomScale)));
		// bottomMaterial.SetFloat("_TickThickness", _tickThickness * (bottomScale * (1 - topScale)));
		float topThickness =_tickThickness * (_tickThickness / (topScale * _tickThickness));
		float bottomThickness =_tickThickness * (_tickThickness / (bottomScale * _tickThickness));
		topMaterial.SetFloat("_TickThickness", topThickness);
		bottomMaterial.SetFloat("_TickThickness", bottomThickness);

		topMaterial.SetFloat("_Transparency", _topTranparency);
		bottomMaterial.SetFloat("_Transparency", _bottomTranparency);
	}
}
