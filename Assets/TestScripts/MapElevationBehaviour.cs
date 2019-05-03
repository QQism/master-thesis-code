using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class MapElevationBehaviour : MonoBehaviour {

	[SerializeField]
	public GameObject _cylinder;
	public GameObject _sphere;
	private AbstractMap _map;

	void Awake()
	{
		_map = transform.GetComponentInChildren<AbstractMap>();
		_map.OnInitialized += onLoadMap;
		_map.OnUpdated += onLoadMap;
	}
	
	void Update() {
		//onLoadMap();	
	}

	void onLoadMap()
	{
		var position = new Vector2d(45.374218, -121.688341);

        MapDataPoint point = new MapDataPoint();
        point.GeoPosition = position;
        var elevation = _map.QueryElevationInUnityUnitsAt(point.GeoPosition);
        point.WorldPosition = _map.GeoToWorldPosition(position, true);

		//_cylinder.transform.position = point.WorldPosition;
        //_cylinder.transform.position = new Vector3(point.WorldPosition.x,
        //                                            _cylinder.transform.localScale.y,
        //                                            point.WorldPosition.z);

		_sphere.transform.position = point.WorldPosition;
        _sphere.transform.position = new Vector3(point.WorldPosition.x,
                                                    _sphere.transform.localScale.y,
                                                    point.WorldPosition.z);

		Debug.Log("Mt hood World Position: " + point.WorldPosition);
	}	
}
