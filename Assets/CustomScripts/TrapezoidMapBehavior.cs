using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezoidMapBehavior : MonoBehaviour {

	[Header("Shape Transformations")]
	[Range(0, 5)]
	public float _upperScale = 1.0f;
	[Range(0, 5)]
	public float _lowerScale = 1.0f;

	private GameObject _quad;

	[Header("Main Colors")]
	[Range(0, 1)]
	public float _transparency;

	public float _angle = 0;

	public float _no = 0;

	public float _miterAngle = 90;

	public int _quadsCount = 0;

	void Awake()
	{
		GameObject container = transform.Find("RotationContainer").gameObject;
		_quad = container.transform.Find("Quad").gameObject;

		// Create copies of materials 
		Renderer renderer =  _quad.GetComponent<Renderer>();

        Material material = new Material(renderer.sharedMaterial);

		renderer.sharedMaterial = material;
	}

	void OnValidate()
	{
		ReCalculateScale();
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void ReCalculateScale()
	{
		if (_quad == null)
			return;

		Material material =  _quad.GetComponent<Renderer>().sharedMaterial;

		if (material == null)
			return;

		float midScale = _upperScale + (_lowerScale -_upperScale);
		material.SetFloat("_UpperScale", _upperScale);
		material.SetFloat("_LowerScale", midScale);

		material.SetFloat("_Transparency", _transparency);

		material.SetFloat("_RotationAngle", _angle * _no);
		material.SetFloat("_MiterAngle", _miterAngle);
		material.SetInt("_QuadsCount", _quadsCount);
	}

	public void addObjectOnSurface(GameObject obj, float angle, float distance)
	{
		obj.transform.SetParent(_quad.transform);
        obj.transform.localPosition = new Vector3(
			distance * Mathf.Cos(angle * Mathf.Deg2Rad), 
			obj.transform.localScale.y / 2, 
			distance * Mathf.Sin(angle * Mathf.Deg2Rad));
	}
}
