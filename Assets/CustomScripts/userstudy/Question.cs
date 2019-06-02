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
	// Estimation
	Task1TrainingDataset,
	Task2TrainingDataset,
	Task3TrainingDataset,
	Dataset1,
	// Higher 1
	Dataset2,
	// Higher 2
	//Dataset3,
	// Higher 3
	//Dataset4,
	// Closer 1
	Dataset5,
	// Closer 2
	//Dataset6,
	// Closer 3
	//Dataset7,
}

public enum ParticipantGroup
{
	Group1,
	Group2,
	Group3,
}
public class Question
{
	public int id;
	public string code;
	public Task task;
	public VisualisationType visualisationType;
	//public List<MapDataPoint> dataPoints;
	public Dataset dataset;
	public int dataPoint1Idx = -1;
	public int dataPoint2Idx = -1;

	public Question(int id, string code, Task task, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx)
	{
		this.id = id;
		this.code = code;
		this.task = task;
		this.visualisationType = visualisationType;	
		this.dataset = dataset;
		this.dataPoint1Idx = dataPoint1Idx;
		this.dataPoint2Idx = dataPoint2Idx;
	}

	public static Question createEstimate(int id, string code, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx) 
	{
		return new Question(id, code, Task.EstimateSinglePoint, visualisationType, dataset, dataPoint1Idx, -1);
	}

	public static Question createLarger(int id, string code, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx) 
	{
		return new Question(id, code, Task.PickLargerDataPoint, visualisationType, dataset, dataPoint1Idx, dataPoint2Idx);
	}

	public static Question createCloser(int id, string code, VisualisationType visualisationType, Dataset dataset, int dataPoint1Idx, int dataPoint2Idx) 
	{
		return new Question(id, code, Task.PickCloserDataPoint, visualisationType, dataset, dataPoint1Idx, dataPoint2Idx);
	}

    public MapDataPoint getDataPoint1()
    {
        return DataPointsManager.Instance.mapDataPoints[dataPoint1Idx];
    }

    public MapDataPoint getDataPoint2()
    {
        if (dataPoint2Idx >= 0)
        {
            return DataPointsManager.Instance.mapDataPoints[dataPoint2Idx];
        }
        else
        {
            return null;
        }
    }

    /* If est question, would be in [0, 100]
	 * If option question, would be in [1, 2]
	 */
    public int getAnswer()
    {
		int answer = 0;
        switch (task)
        {
            case Task.EstimateSinglePoint:
                var dataPoint = getDataPoint1();
                answer = Mathf.RoundToInt(dataPoint.Value * 100f);
                break;
            case Task.PickLargerDataPoint:
                var dataPointL1 = getDataPoint1();
                var dataPointL2 = getDataPoint2();

                if (dataPointL1.Value > dataPointL2.Value)
                {
                    answer = BarOption.Bar1;
                }
                else
                {
                    answer = BarOption.Bar2;
                }
                break;
            case Task.PickCloserDataPoint:
                var dataPointC1 = getDataPoint1();
                var dataPointC2 = getDataPoint2();

                if (dataPointC1.RawPosition.magnitude < dataPointC2.RawPosition.magnitude)
                {
                    answer = BarOption.Bar1;
                }
                else
                {
                    answer = BarOption.Bar2;
                }
                break;
        }

        return answer;
    }
}
