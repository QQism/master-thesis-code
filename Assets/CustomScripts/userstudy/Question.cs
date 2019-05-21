using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Task
{
	EstimateSinglePoint,
	PickLargerDataPoint,
	PickCloserDataPoint,
}

public enum Dataset
{
	None,
	Dataset1,
	Dataset2,
	Dataset3,
}
public class Question
{
	public int id;
	public Task task;
	public VisualisationType visualisationType;
	//public List<MapDataPoint> dataPoints;
	public Dataset dataset;
	public int dataPoint1Idx;
	public int dataPoint2Idx;
	public int dataPoint3Idx;

	public Question(int id, Task task, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx)
	{
		this.id = id;
		this.task = task;
		this.visualisationType = visualisationType;	
		this.dataset = dataset;
		this.dataPoint1Idx = dataPoint1Idx;
		this.dataPoint2Idx = dataPoint2Idx;
	}

	public static Question createEstimate(int id, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx) 
	{
		return new Question(id, Task.EstimateSinglePoint, visualisationType, dataset, dataPoint1Idx, -1);
	}

	public static Question createLarger(int id, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx) 
	{
		return new Question(id, Task.PickLargerDataPoint, visualisationType, dataset, dataPoint1Idx, dataPoint2Idx);
	}

	public static Question createCloser(int id, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx) 
	{
		return new Question(id, Task.PickCloserDataPoint, visualisationType, dataset, dataPoint1Idx, dataPoint2Idx);
	}

	/* If est question, would be in [0, 100]
	 * If option question, would be in [1, 2]
	 */
	public int getAnswer()
	{
		return 0;
	}
}
