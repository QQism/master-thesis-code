using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConeTransformBehavior : MonoBehaviour {

	public float _coneAngle = 30;

	public float _originRotation = 0;
	// Use this for initialization
	void Start () {
		//float distance = transform. Mathf.Tan(Mathf.Deg2Rad * _coneAngle);

		transform.Rotate(_coneAngle, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
