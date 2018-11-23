using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarCustomData : MonoBehaviour {
    public float _value;
    public Vector2d _latLong;

    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public Vector2d LatLong
    {
        get
        {
            return _latLong;
        }
        set
        {
            _latLong = value;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
