using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAdjustment : MonoBehaviour {
	public Camera playerCamera;
	public bool _frontFacing = true;
	private Quaternion _originRotation;
	
	void Awake()
	{
		_originRotation = transform.rotation;
	}

	void LateUpdate () {
		if (_frontFacing)
			transform.rotation = playerCamera.transform.rotation;
		else
			transform.rotation = _originRotation;
	}
}
