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
	//public List<MapDataPoint> dataPoints;
	public int dataPoint1Idx;
	public int dataPoint2Idx;
	public int dataPoint3Idx;

	public Question(int id, Task task, VisualisationType visualisationType, int dataPoint1Idx): this(id, task, visualisationType, dataPoint1Idx, -1)
	{
		
	}
	public Question(int id, Task task, VisualisationType visualisationType, int dataPoint1Idx, int dataPoint2Idx)
	{
		this.id = id;
		this.task = task;
		this.visualisationType = visualisationType;	
		this.dataPoint1Idx = dataPoint1Idx;
		this.dataPoint2Idx = dataPoint2Idx;
	}
}
