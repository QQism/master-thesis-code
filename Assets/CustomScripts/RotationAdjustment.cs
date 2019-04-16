using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAdjustment : MonoBehaviour {
	public bool _lookingAtCamera = true;
	private Quaternion _originRotation;
	
	void Awake()
	{
		_originRotation = transform.rotation;
	}

	void LateUpdate () {
		if (_lookingAtCamera)
			transform.rotation = Camera.main.transform.rotation;
		else
			transform.rotation = _originRotation;
	}
}
