using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttachedBehavior : MonoBehaviour {

	[Range(0, 1)]
	public float _coneHeight = 0.95f;
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Scale(Camera.main.transform.position, new Vector3(1, _coneHeight, 1));
	}
}