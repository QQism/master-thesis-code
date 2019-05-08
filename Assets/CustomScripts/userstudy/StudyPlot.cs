using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyPlot
{

	private static StudyPlot instance = null;
	private static readonly object padlock = new object();

	private List<Question> _quesitons {get; set;}

	private int _currentQuestionId;

	public int userId;

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
	StudyPlot()
	{
		_quesitons = new List<Question>() {
			new Question(1, Task.EstimateSinglePoint, VisualisationType.MapCone, 10),
			new Question(2, Task.PickLargerDataPoint, VisualisationType.MapCone, 10, 20)
		};
	}

	public void setStartQuestionIdx(int questionIdx)
	{
		_currentQuestionId = questionIdx;
	}

	public void startResponse()
	{

	}

	public Question currentQuestion()
	{
		return _quesitons[_currentQuestionId];
	}

	public Question nextQuestion()
	{
		if (_currentQuestionId >= _quesitons.Count)
			return null;

		return _quesitons[++_currentQuestionId];
	}

	public UserResponse nextResponse()
	{
		return null;
	}
}
