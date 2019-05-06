﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMarkerBehavior : MonoBehaviour {

	public bool _selected = false;
	public GameObject _markerCylinder;
	public GameObject _indicationArrow;

	public float _animationSpeed = 6.0f;
	public float _textAnimationThreshold = 2.5f;

	private RectTransform _rectTextTransform;

	private Vector3 _startTextPosition;

	private Material _cylinderMaterial;
	// Use this for initialization

	void Awake() {
		_rectTextTransform = _indicationArrow.GetComponent<RectTransform>();

		// Clone a material, so it won't affect other cylinder using the same material
		Renderer cylinderRenderer = _markerCylinder.GetComponent<Renderer>();
		_cylinderMaterial = new Material(cylinderRenderer.sharedMaterial);
		cylinderRenderer.sharedMaterial = _cylinderMaterial;
		_startTextPosition = _rectTextTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		_indicationArrow.transform.LookAt(Camera.main.transform);
		if (_selected) {
			_indicationArrow.SetActive(true);
			Vector3 moving = new Vector3(0, Mathf.Sin(_animationSpeed * Time.time)/_textAnimationThreshold + 2.2f, 0);
			_rectTextTransform.position = _startTextPosition + moving;
			_cylinderMaterial.SetInt("_OutlineOn", 1);
			_cylinderMaterial.SetFloat("_OutlineWidth", 1+Mathf.Sin(_animationSpeed * Time.time)/5);
		} else {
			_indicationArrow.SetActive(false);
			_cylinderMaterial.SetInt("_OutlineOn", 0);
		}	
	}
}