using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttachedBehavior : MonoBehaviour {

	public Camera _mainCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Scale(_mainCamera.transform.position, new Vector3(1, 0.95f, 1));
	}
}
