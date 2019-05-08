using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Task
{
	EstimateSinglePoint,
	PickLargerDataPoint
}
public class Question
{
	public int id;
	public Task task;
	public VisualisationType visualisationType;
	public List<MapDataPoint> dataPoints;
	public MapDataPoint dataPoint1;
	public MapDataPoint dataPoint2;
	public MapDataPoint dataPoint3;
}
