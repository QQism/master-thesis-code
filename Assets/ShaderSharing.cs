using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSharing : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponentInChildren<MeshRenderer>().sharedMaterials = GetComponent<MeshRenderer>().materials;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
