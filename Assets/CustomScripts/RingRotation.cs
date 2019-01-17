using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRotation : MonoBehaviour {

	[Range(0, 90)]
	public float _angle = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnValidate()
	{
		for (int i=0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			Debug.Log(child.localPosition);
			Debug.Log(Vector3.Angle(child.localPosition, Vector3.up)); 

			//child.localPosition = Quatern(_angle, Vector3.right) * child.localPosition;
			//child.localPosition = Quaternion.AngleAxis(_angle, Vector3.right) * child.localPosition;
		}
	}


}
