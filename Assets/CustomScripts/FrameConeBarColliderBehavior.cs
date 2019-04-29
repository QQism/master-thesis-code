using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameConeBarColliderBehavior : MonoBehaviour {

	void Awake()
	{
		var boxCollider = gameObject.GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;		
	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("OnTriggerEnter");
		//gameObject
	}

	private void OnTriggerExit(Collider other)
	{	
		//Debug.Log("OnTriggerExit");
	}
}
