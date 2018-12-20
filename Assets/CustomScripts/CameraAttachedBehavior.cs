using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttachedBehavior : MonoBehaviour {

	public Camera _mainCamera;

	[Range(0, 1)]
	public float _coneHeight = 0.95f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Scale(_mainCamera.transform.position, new Vector3(1, _coneHeight, 1));
	}
}