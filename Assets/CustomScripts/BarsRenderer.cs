using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsRenderer : MonoBehaviour {

	public GameObject _quad;

	[Range(0, 90)]
	public float _miterAngle = 45;

	private int _quadsCount = 4;

	private float _faceHeight = 1.0f;

	private List<GameObject> bars = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
		float rotateYAngle = 360.0f / _quadsCount;
        for (int i = 0; i < _quadsCount; i++)
        {
            GameObject bar = Instantiate(_quad, transform.position, Quaternion.identity);
			bar.name = "Bar " + i.ToString();
            bar.transform.SetParent(transform);
            Material quadMaterial = new Material(_quad.GetComponent<Renderer>().sharedMaterial);
            bar.GetComponent<Renderer>().sharedMaterial = quadMaterial;

			float miterRadAngle = _miterAngle * Mathf.Deg2Rad;
			float baseRadius = _faceHeight * Mathf.Sin(miterRadAngle);
			float upperScale = baseRadius * 2;
			float height = _faceHeight * Mathf.Sin(Mathf.PI/2 - miterRadAngle);


            quadMaterial.SetFloat("_UpperScale", upperScale);
            quadMaterial.SetFloat("_LowerScale", 0.0f);
			bar.transform.localPosition += bar.transform.TransformDirection(bar.transform.forward) * baseRadius/2.0f;
			bar.transform.RotateAround(Vector3.zero, Vector3.up, i * rotateYAngle);
			bar.transform.Rotate(Vector3.right * _miterAngle, Space.Self);
        }
	}

	// Update is called once per frame
	void Update () {
		
	}
}
