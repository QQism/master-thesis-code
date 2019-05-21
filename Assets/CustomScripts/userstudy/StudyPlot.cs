using System.Collections;
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
			Question.createEstimate(0, VisualisationType.MapCone, Dataset.Dataset1, 10),
			Question.createLarger(1, VisualisationType.BarCone, Dataset.Dataset1, 10, 20),
			Question.createCloser(2, VisualisationType.InPlaceBars, Dataset.Dataset1, 8, 43),
			Question.createLarger(3, VisualisationType.BarCone, Dataset.Dataset1, 22, 33),
			Question.createEstimate(4, VisualisationType.MapCone, Dataset.Dataset1, 50),
			Question.createCloser(5, VisualisationType.InPlaceBars, Dataset.Dataset1, 44, 80),
			Question.createEstimate(6, VisualisationType.BarCone, Dataset.Dataset1, 87),
			Question.createLarger(7, VisualisationType.MapCone, Dataset.Dataset1, 86, 94),
			Question.createCloser(8, VisualisationType.InPlaceBars, Dataset.Dataset1, 20, 39),
			Question.createEstimate(9, VisualisationType.BarCone, Dataset.Dataset1, 55),
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
