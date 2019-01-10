using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public class MapDataPoint {
	public string Name {get; set;}
	public Vector2d GeoPosition {get; set;}
	public Vector3 WorldPosition {get; set;}
	public float Value {get; set;}
}
