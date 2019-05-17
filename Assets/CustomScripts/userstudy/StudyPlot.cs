﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlotState
{
	NotStarted, // start state
	OnDoingQuestion, // waiting the answer
	OnCompletedQuestion, // move to the next question
	OnFinished // there is not question left, Finished
}

public class StudyPlot
{
	private static StudyPlot instance = null;
	private static readonly object padlock = new object();

	private List<Question> _quesitons {get; set;}

	public PlotState state = PlotState.NotStarted;

	private int _currentQuestionId;

	private List<UserResponse> _allResponses;
	private UserResponse _currentResponse;

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
		_allResponses = new List<UserResponse>();
		_quesitons = new List<Question>() {
			new Question(1, Task.EstimateSinglePoint, VisualisationType.MapCone, 10),
			new Question(2, Task.PickLargerDataPoint, VisualisationType.BarCone, 10, 20),
			new Question(3, Task.EstimateSinglePoint, VisualisationType.InPlaceBars, 8),
			new Question(4, Task.PickLargerDataPoint, VisualisationType.BarCone, 22, 33),
			new Question(5, Task.EstimateSinglePoint, VisualisationType.MapCone, 50),
			new Question(6, Task.PickLargerDataPoint, VisualisationType.InPlaceBars, 44, 80),
			new Question(7, Task.EstimateSinglePoint, VisualisationType.BarCone, 87),
			new Question(8, Task.PickLargerDataPoint, VisualisationType.MapCone, 86, 94),
		};
	}

	public void setStartQuestionIdx(int questionIdx)
	{
		_currentQuestionId = questionIdx;
	}

	public Question startPlot()
	{
		if (_currentQuestionId >= _quesitons.Count)
			return null;

		var nextQuestion = _quesitons[_currentQuestionId];
		_currentResponse = new UserResponse(nextQuestion);
		_allResponses.Add(_currentResponse);

		state = PlotState.OnDoingQuestion;

		return nextQuestion;
	}

	public void answer(int value)
	{
		_currentResponse.answer = value;
		_currentResponse.completionTime = Time.time;
		_currentResponse.save();

		if (_currentQuestionId >= _quesitons.Count-1)
		{
			state = PlotState.OnFinished;
		}
        else
        {
            state = PlotState.OnCompletedQuestion;
        }
	}

	public Question currentQuestion()
	{
		return _quesitons[_currentQuestionId];
	}

	public Question nextQuestion()
	{
		if (_currentQuestionId >= _quesitons.Count-1)
		{
			state = PlotState.OnFinished;
			return null;
		}

		var nextQuestion = _quesitons[++_currentQuestionId];
		_currentResponse = new UserResponse(nextQuestion);
		_allResponses.Add(_currentResponse);

		_currentResponse.startTime = Time.time;
		state = PlotState.OnDoingQuestion;

		return nextQuestion;
	}
}
