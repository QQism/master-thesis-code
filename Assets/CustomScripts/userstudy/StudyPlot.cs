using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyPlot
{

	private static StudyPlot instance = null;
	private static readonly object padlock = new object();

	public List<MapDataPoint> mapDataPoints {get; set;}
	public float maxValue {get; set;}

	StudyPlot()
	{
	}

	public static StudyPlot Instance
	{
		get
		{
			lock(padlock)
			{
				if (instance == null)
					instance = new StudyPlot();

				return instance;
			}
		}
	}

	public void setStartQuestionIndex(int questionId)
	{

	}

	public Question nextQuestion()
	{
		return null;
	}

	public UserResponse nextResponse()
	{
		return null;
	}
}
