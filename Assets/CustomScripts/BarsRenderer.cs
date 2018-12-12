using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsRenderer : MonoBehaviour {

	public GameObject _quad;

	private List<GameObject> bars = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
		GameObject bar = Instantiate(_quad, Vector3.zero, Quaternion.identity); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
