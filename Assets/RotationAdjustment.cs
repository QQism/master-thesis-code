using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAdjustment : MonoBehaviour {
	public Camera playerCamera;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.rotation = playerCamera.transform.rotation;
	}
}
