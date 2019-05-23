using System.Collections;
using System.Collections.Generic;
using System;
using Mapbox.Utils;
using UnityEngine;
using Mapbox.Unity.Map;

public class MapDataPoint {
	public string Name {get; set;}

	private Vector2 _rawPosition;
	public Vector2 RawPosition
	{
		get
		{
			return _rawPosition;
		}
		set 
		{
			_rawPosition = value;

			DataPointsManager manager = DataPointsManager.Instance;
			AbstractMap map = manager.Map;

            float maxX = manager.MapMaxX,
            maxY = manager.MapMaxY,
            minX = manager.MapMinX,
            minY = manager.MapMinY;

            float x = ((_rawPosition.x + 1) * (Mathf.Abs(maxX) + Mathf.Abs(minX)) / 2) - Mathf.Abs(minX);
            float z = ((_rawPosition.y + 1) * (Mathf.Abs(maxY) + Mathf.Abs(minY)) / 2) - Mathf.Abs(minY);

            Vector2d latLong = map.WorldToGeoPosition(new Vector3(x, 0, z));
            float height = map.QueryElevationInUnityUnitsAt(latLong);

			GeoPosition = latLong;
			WorldPosition = new Vector3(x, height, z);
		}
	}

	public Vector2d GeoPosition {get; set;}
	public Vector3 WorldPosition {get; set;} 
	public Vector2 ConePosition {get; set;}
	public float Value {get; set;}

    public bool Selected { get; set; }

    public event Action OnPoseEnter = delegate { };
    public event Action OnPoseLeave = delegate { };
    public event Action OnQuestionOption1ToShow = delegate { };
    public event Action OnQuestionOption2ToShow = delegate { };

	public event Action OnQuestionOption1Completed = delegate { };
	public event Action OnQuestionOption2Completed = delegate { };

	public event Action OnRawPositionChanged = delegate {};

	public void poseEnter()
	{
		OnPoseEnter();
	}

	public void poseLeave()
	{
		OnPoseLeave();
	}

	public void showQuestionOption1()
	{
		OnQuestionOption1ToShow();
	}

	public void showQuestionOption2()
	{
		OnQuestionOption2ToShow();
	}

	public void completeQuestionOption1()
	{
		OnQuestionOption1Completed();
	}

	public void completeQuestionOption2()
	{
		OnQuestionOption2Completed();
	}

	public enum MapDataPointState 
	{
		Question1,
		Question2,
		NotQuestion
	}
}
