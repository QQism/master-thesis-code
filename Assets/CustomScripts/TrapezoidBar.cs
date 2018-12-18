using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezoidBar : MonoBehaviour {

	[Range(0, 5)]
	public float _upperScale = 1.0f;
	[Range(0, 5)]
	public float _lowerScale = 1.0f;

	[Range(0, 1)]
	public float _level = 0.5f;
	private GameObject _topBar;
	private GameObject _bottomBar;
	// Use this for initialization

	void Start () { 
		_topBar = transform.Find("Top").gameObject;
		_bottomBar = transform.Find("Bottom").gameObject;

		// Create copies of materials 
		Renderer topRenderer =  _topBar.GetComponent<Renderer>();
		Renderer bottomRenderer =  _bottomBar.GetComponent<Renderer>();

        Material topMaterial = new Material(topRenderer.sharedMaterial);
        Material bottomMaterial = new Material(bottomRenderer.sharedMaterial);

		topRenderer.sharedMaterial = topMaterial;
		bottomRenderer.sharedMaterial = bottomMaterial;
	}

	void Awake()
	{
	}

	void OnValidate()
	{
		if (_topBar == null || _bottomBar == null)
			return;

		_bottomBar.transform.localScale.Set(1, 1, _level);
		_topBar.transform.localScale.Set(1, 1, 1 - _level);

		Vector3 oldBottomPosition = _bottomBar.transform.position;
		Vector3 oldTopPosition = _topBar.transform.position;

		_bottomBar.transform.position.Set(oldBottomPosition.x, oldBottomPosition.y, _level - 1);
		_topBar.transform.position.Set(oldTopPosition.x, oldTopPosition.y, 1 - _level);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
