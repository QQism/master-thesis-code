using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsRenderer : MonoBehaviour {

	public GameObject _quad;

	private int _quadsCount = 4;

	private List<GameObject> bars = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
		float rotateAngle = 360.0f / _quadsCount;
        for (int i = 0; i < 4; i++)
        {
            GameObject bar = Instantiate(_quad, transform.position, Quaternion.identity);
			bar.name = "Bar " + i.ToString();
            bar.transform.SetParent(transform);
            Material quadMaterial = new Material(_quad.GetComponent<Renderer>().sharedMaterial);
            bar.GetComponent<Renderer>().sharedMaterial = quadMaterial;
            quadMaterial.SetFloat("_UpperScale", 1.0f);
            quadMaterial.SetFloat("_LowerScale", 0.0f);
            bar.transform.Rotate(0, i*rotateAngle, 0);
			bar.transform.Rotate(63.434f, 0, 0);
			bar.transform.localPosition += new Vector3(0, 0, 0.5f);
        }
	}

	float calculateUpperScale(float S, float alphaAngle)
	{
		return Mathf.Sqrt(2 * Mathf.Pow(S, 2) * (1 - Mathf.Cos(alphaAngle)));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
