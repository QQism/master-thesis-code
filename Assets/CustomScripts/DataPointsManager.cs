using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPointsManager {

	private static DataPointsManager instance = null;
	private static readonly object padlock = new object();

	public List<MapDataPoint> mapDataPoints {get; set;}
	public List<MapDataPoint> mapTestDataPoints {get; set;}
	public float maxValue {get; set;}
	public float testMaxValue {get; set;}

	DataPointsManager()
	{
		initializeData();
	}

	public static DataPointsManager Instance
	{
		get
		{
			lock(padlock)
			{
				if (instance == null)
					instance = new DataPointsManager();

				return instance;
			}
		}
	}

	void initializeData()
	{
		mapDataPoints = new List<MapDataPoint>();
		mapTestDataPoints = new List<MapDataPoint>();
	}

	public void resetPosing()
	{
		foreach(MapDataPoint point in mapDataPoints)
		{
			point.poseLeave();
		}
	}
}
