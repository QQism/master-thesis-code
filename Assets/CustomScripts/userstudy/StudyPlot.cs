using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum PlotState
{
	NotStarted, // start state
	OnDoingQuestion, // waiting the answer
	OnCompletedQuestion, // move to the next question
	ChangeDataset,
	OnFinished // there is not question left, Finished
}

public class StudyPlot
{
	private static StudyPlot instance = null;
	private static readonly object padlock = new object();

	public List<Question> _questions {get; set;}
	private string _currentDatasetFile;

	private Dataset _currentDataset;

	public PlotState state = PlotState.NotStarted;

	private int _currentQuestionId;

	private List<UserResponse> _allResponses;
	private UserResponse _currentResponse;

	public int userId;

	public Dictionary<string, List<Question>> datasetQuestions;
	public Dictionary<Dataset, string> datasetFiles;

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
		setUpQuestions();
	}

	public void setStartQuestionIdx(int questionIdx)
	{
		_currentQuestionId = questionIdx;
	}

	public Question startPlot()
	{
		if (_currentQuestionId >= _questions.Count)
			return null;

		var nextQuestion = _questions[_currentQuestionId];
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

		//if (_currentQuestionId >= _questions.Count-1)
		//{
		//	state = PlotState.OnFinished;
		//}
        //else
        //{
            state = PlotState.OnCompletedQuestion;
        //}
	}

	public Question currentQuestion()
	{
		return _questions[_currentQuestionId];
	}

	public Question nextQuestion()
	{
		if (_currentQuestionId >= _questions.Count-1)
		{
            if (getNextDataset() == Dataset.None)
            {
                state = PlotState.OnFinished;
            }
            else
            {
                state = PlotState.ChangeDataset;
            }
			return null;
		}

		var nextQuestion = _questions[++_currentQuestionId];
		_currentResponse = new UserResponse(nextQuestion);
		_allResponses.Add(_currentResponse);

		_currentResponse.startTime = Time.time;
		state = PlotState.OnDoingQuestion;

		return nextQuestion;
	}

	public void setDataset(Dataset dataset, ParticipantGroup group) {
		_currentDataset = dataset;
		_currentDatasetFile = datasetFiles[_currentDataset];

		_questions = datasetQuestions[getDatasetKey(dataset, group)];
	}

	void setUpQuestions()
	{
		datasetFiles = new Dictionary<Dataset, string>();
		datasetFiles.Add(Dataset.Dataset1, "dataset_est_latest.csv");

		datasetFiles.Add(Dataset.Dataset2, "dataset_higher1_latest.csv");
		datasetFiles.Add(Dataset.Dataset3, "dataset_higher2_latest.csv");
		datasetFiles.Add(Dataset.Dataset4, "dataset_higher3_latest.csv");

		datasetFiles.Add(Dataset.Dataset5, "dataset_closer1_latest.csv");
		datasetFiles.Add(Dataset.Dataset6, "dataset_closer2_latest.csv");
		datasetFiles.Add(Dataset.Dataset7, "dataset_closer3_latest.csv");

		datasetQuestions = new Dictionary<string, List<Question>>();

		setUpDataset1();
		setUpDataset2();
		setUpDataset3();
		setUpDataset4();
		setUpDataset5();
		setUpDataset6();
		setUpDataset7();
	}

	public string getDatasetFile()
	{
        return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            "Datasets",
            _currentDatasetFile
        });
	}

	public Dataset getNextDataset()
	{
		Dataset nextDataset = Dataset.None;

		switch(_currentDataset)
		{
            case Dataset.Dataset2:
                nextDataset = Dataset.Dataset3;
                break;
            case Dataset.Dataset3:
                nextDataset = Dataset.Dataset4;
                break;
            case Dataset.Dataset5:
                nextDataset = Dataset.Dataset6;
                break;
            case Dataset.Dataset6:
                nextDataset = Dataset.Dataset7;
                break;
            case Dataset.Dataset1:
            case Dataset.Dataset4:
            case Dataset.Dataset7:
			default:
                break;
        }

		return nextDataset;
	}

	void setUpDataset1()
	{
        datasetQuestions.Add(getDatasetKey(Dataset.Dataset1, ParticipantGroup.Group1), new List<Question>() {
            Question.createEstimate(0, VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(1, VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(2, VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(3, VisualisationType.InPlaceBars, Dataset.Dataset1, 3),

            Question.createEstimate(4, VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(5, VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(6, VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(7, VisualisationType.BarCone, Dataset.Dataset1, 7),

            Question.createEstimate(8, VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(9, VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(10, VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(11, VisualisationType.MapCone, Dataset.Dataset1, 11),
        });

		string group2Dataset1 = ParticipantGroup.Group2.ToString() + "_" + Dataset.Dataset1.ToString();

        datasetQuestions.Add(group2Dataset1, new List<Question>() {
            Question.createEstimate(0, VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(1, VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(2, VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(3, VisualisationType.BarCone, Dataset.Dataset1, 7),

            Question.createEstimate(4, VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(5, VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(6, VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(7, VisualisationType.MapCone, Dataset.Dataset1, 11),

            Question.createEstimate(8, VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(9, VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(10, VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(11, VisualisationType.InPlaceBars, Dataset.Dataset1, 3),
        });

		string group3Dataset1 = ParticipantGroup.Group3.ToString() + "_" + Dataset.Dataset1.ToString();

        datasetQuestions.Add(group3Dataset1, new List<Question>() {
            Question.createEstimate(0, VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(1, VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(2, VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(3, VisualisationType.MapCone, Dataset.Dataset1, 11),

            Question.createEstimate(4, VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(5, VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(6, VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(7, VisualisationType.InPlaceBars, Dataset.Dataset1, 3),

            Question.createEstimate(8, VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(9, VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(10, VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(11, VisualisationType.BarCone, Dataset.Dataset1, 7),
        });
	}

	void setUpDataset2()
	{
		string group1Dataset2 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset2.ToString();

		datasetQuestions.Add(group1Dataset2, new List<Question>() {
			/*
			// 1
			Question.createLarger(0, VisualisationType.InPlaceBars, Dataset.Dataset2, 25, 26),
			Question.createLarger(1, VisualisationType.BarCone, Dataset.Dataset2, 43, 44),
			Question.createLarger(2, VisualisationType.MapCone, Dataset.Dataset2, 85, 86),
			// 2
			Question.createLarger(3, VisualisationType.BarCone, Dataset.Dataset2, 67, 68),
			Question.createLarger(4, VisualisationType.MapCone, Dataset.Dataset2, 73, 74),
			Question.createLarger(5, VisualisationType.InPlaceBars, Dataset.Dataset2, 1, 2),
			// 3
			Question.createLarger(6, VisualisationType.MapCone, Dataset.Dataset2, 103, 104),
			Question.createLarger(7, VisualisationType.InPlaceBars, Dataset.Dataset2, 19, 20),
			Question.createLarger(8, VisualisationType.BarCone, Dataset.Dataset2, 49, 50),
			// 4
			Question.createLarger(9, VisualisationType.InPlaceBars, Dataset.Dataset2, 7, 8),
			Question.createLarger(10, VisualisationType.BarCone, Dataset.Dataset2, 61, 62),
			Question.createLarger(11, VisualisationType.MapCone, Dataset.Dataset2, 97, 98),
			// 5
			Question.createLarger(12, VisualisationType.BarCone, Dataset.Dataset2, 55, 56),
			Question.createLarger(13, VisualisationType.MapCone, Dataset.Dataset2, 91, 92),
			Question.createLarger(14, VisualisationType.InPlaceBars, Dataset.Dataset2, 13, 14),
			// 6
			Question.createLarger(15, VisualisationType.MapCone, Dataset.Dataset2, 79, 80),
			Question.createLarger(16, VisualisationType.InPlaceBars, Dataset.Dataset2, 31, 32),
			Question.createLarger(17, VisualisationType.BarCone, Dataset.Dataset2, 37, 38),
			// 7
			Question.createLarger(18, VisualisationType.InPlaceBars, Dataset.Dataset2, 15, 16),
			Question.createLarger(19, VisualisationType.BarCone, Dataset.Dataset2, 57, 58),
			Question.createLarger(20, VisualisationType.MapCone, Dataset.Dataset2, 81, 82),
			// 8
			Question.createLarger(21, VisualisationType.BarCone, Dataset.Dataset2, 39, 40),
			Question.createLarger(22, VisualisationType.MapCone, Dataset.Dataset2, 99, 100),
			Question.createLarger(23, VisualisationType.InPlaceBars, Dataset.Dataset2, 33, 34),
			// 9
			Question.createLarger(24, VisualisationType.MapCone, Dataset.Dataset2, 93, 94),
			Question.createLarger(25, VisualisationType.InPlaceBars, Dataset.Dataset2, 3, 4),
			Question.createLarger(26, VisualisationType.BarCone, Dataset.Dataset2, 63, 64),
			// 10
			Question.createLarger(27, VisualisationType.InPlaceBars, Dataset.Dataset2, 27, 28),
			Question.createLarger(28, VisualisationType.BarCone, Dataset.Dataset2, 45, 46),
			Question.createLarger(29, VisualisationType.MapCone, Dataset.Dataset2, 87, 88),
			// 11
			Question.createLarger(30, VisualisationType.BarCone, Dataset.Dataset2, 69, 70),
			Question.createLarger(31, VisualisationType.MapCone, Dataset.Dataset2, 75, 76),
			Question.createLarger(32, VisualisationType.InPlaceBars, Dataset.Dataset2, 9, 10),
			// 12
			Question.createLarger(33, VisualisationType.MapCone, Dataset.Dataset2, 105, 106),
			Question.createLarger(34, VisualisationType.InPlaceBars, Dataset.Dataset2, 21, 22),
			Question.createLarger(35, VisualisationType.BarCone, Dataset.Dataset2, 51, 52),
			// 13
			Question.createLarger(36, VisualisationType.InPlaceBars, Dataset.Dataset2, 5, 6),
			Question.createLarger(37, VisualisationType.BarCone, Dataset.Dataset2, 71, 72),
			Question.createLarger(38, VisualisationType.MapCone, Dataset.Dataset2, 107, 108),
			// 14
			Question.createLarger(39, VisualisationType.BarCone, Dataset.Dataset2, 53, 54),
			Question.createLarger(40, VisualisationType.MapCone, Dataset.Dataset2, 89, 90),
			Question.createLarger(41, VisualisationType.InPlaceBars, Dataset.Dataset2, 23, 24),
			// 15
			Question.createLarger(42, VisualisationType.MapCone, Dataset.Dataset2, 77, 78),
			Question.createLarger(43, VisualisationType.InPlaceBars, Dataset.Dataset2, 29, 30),
			Question.createLarger(44, VisualisationType.BarCone, Dataset.Dataset2, 47, 48),
			// 16
			Question.createLarger(45, VisualisationType.InPlaceBars, Dataset.Dataset2, 17, 18),
			Question.createLarger(46, VisualisationType.BarCone, Dataset.Dataset2, 59, 60),
			Question.createLarger(47, VisualisationType.MapCone, Dataset.Dataset2, 83, 84),
			// 17
			Question.createLarger(48, VisualisationType.BarCone, Dataset.Dataset2, 41, 42),
			Question.createLarger(49, VisualisationType.MapCone, Dataset.Dataset2, 101, 102),
			Question.createLarger(50, VisualisationType.InPlaceBars, Dataset.Dataset2, 35, 36),
			// 18
			Question.createLarger(51, VisualisationType.MapCone, Dataset.Dataset2, 95, 96),
			Question.createLarger(52, VisualisationType.InPlaceBars, Dataset.Dataset2, 11, 12),
			Question.createLarger(53, VisualisationType.BarCone, Dataset.Dataset2, 65, 66),
			*/

			// 1
			Question.createLarger(0, VisualisationType.InPlaceBars, Dataset.Dataset2, 10, 11),
			Question.createLarger(1, VisualisationType.BarCone, Dataset.Dataset2, 16, 17),
			Question.createLarger(2, VisualisationType.MapCone, Dataset.Dataset2, 26, 27),
			// 2
			Question.createLarger(3, VisualisationType.BarCone, Dataset.Dataset2, 22, 23),
			Question.createLarger(4, VisualisationType.MapCone, Dataset.Dataset2, 28, 29),
			Question.createLarger(5, VisualisationType.InPlaceBars, Dataset.Dataset2, 0, 1),
			// 3
			Question.createLarger(6, VisualisationType.MapCone, Dataset.Dataset2, 32, 33),
			Question.createLarger(7, VisualisationType.InPlaceBars, Dataset.Dataset2, 6, 7),
			Question.createLarger(8, VisualisationType.BarCone, Dataset.Dataset2, 12, 13),
			// 4
			Question.createLarger(9, VisualisationType.InPlaceBars, Dataset.Dataset2, 2, 3),
			Question.createLarger(10, VisualisationType.BarCone, Dataset.Dataset2, 20, 21),
			Question.createLarger(11, VisualisationType.MapCone, Dataset.Dataset2, 30, 31),
			// 5
			Question.createLarger(12, VisualisationType.BarCone, Dataset.Dataset2, 14, 15),
			Question.createLarger(13, VisualisationType.MapCone, Dataset.Dataset2, 34, 35),
			Question.createLarger(14, VisualisationType.InPlaceBars, Dataset.Dataset2, 4, 5),
			// 6
			Question.createLarger(15, VisualisationType.MapCone, Dataset.Dataset2, 24, 25),
			Question.createLarger(16, VisualisationType.InPlaceBars, Dataset.Dataset2, 8, 9),
			Question.createLarger(17, VisualisationType.BarCone, Dataset.Dataset2, 18, 19),
		});
	}

	void setUpDataset3()
	{
		string group1Dataset3 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset3.ToString();

		datasetQuestions.Add(group1Dataset3, new List<Question>() { 
			// 1
			Question.createLarger(0, VisualisationType.InPlaceBars, Dataset.Dataset3, 10, 11),
			Question.createLarger(1, VisualisationType.BarCone, Dataset.Dataset3, 16, 17),
			Question.createLarger(2, VisualisationType.MapCone, Dataset.Dataset3, 26, 27),
			// 2
			Question.createLarger(3, VisualisationType.BarCone, Dataset.Dataset3, 22, 23),
			Question.createLarger(4, VisualisationType.MapCone, Dataset.Dataset3, 28, 29),
			Question.createLarger(5, VisualisationType.InPlaceBars, Dataset.Dataset3, 0, 1),
			// 3
			Question.createLarger(6, VisualisationType.MapCone, Dataset.Dataset3, 32, 33),
			Question.createLarger(7, VisualisationType.InPlaceBars, Dataset.Dataset3, 6, 7),
			Question.createLarger(8, VisualisationType.BarCone, Dataset.Dataset3, 12, 13),
			// 4
			Question.createLarger(9, VisualisationType.InPlaceBars, Dataset.Dataset3, 2, 3),
			Question.createLarger(10, VisualisationType.BarCone, Dataset.Dataset3, 20, 21),
			Question.createLarger(11, VisualisationType.MapCone, Dataset.Dataset3, 30, 31),
			// 5
			Question.createLarger(12, VisualisationType.BarCone, Dataset.Dataset3, 14, 15),
			Question.createLarger(13, VisualisationType.MapCone, Dataset.Dataset3, 34, 35),
			Question.createLarger(14, VisualisationType.InPlaceBars, Dataset.Dataset3, 4, 5),
			// 6
			Question.createLarger(15, VisualisationType.MapCone, Dataset.Dataset3, 24, 25),
			Question.createLarger(16, VisualisationType.InPlaceBars, Dataset.Dataset3, 8, 9),
			Question.createLarger(17, VisualisationType.BarCone, Dataset.Dataset3, 18, 19),
		});
	}

	void setUpDataset4()
	{
		string group1Dataset4 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset4.ToString();
		datasetQuestions.Add(group1Dataset4, new List<Question>() { 
			// 1
			Question.createLarger(0, VisualisationType.InPlaceBars, Dataset.Dataset4, 10, 11),
			Question.createLarger(1, VisualisationType.BarCone, Dataset.Dataset4, 16, 17),
			Question.createLarger(2, VisualisationType.MapCone, Dataset.Dataset4, 26, 27),
			// 2
			Question.createLarger(3, VisualisationType.BarCone, Dataset.Dataset4, 22, 23),
			Question.createLarger(4, VisualisationType.MapCone, Dataset.Dataset4, 28, 29),
			Question.createLarger(5, VisualisationType.InPlaceBars, Dataset.Dataset4, 0, 1),
			// 3
			Question.createLarger(6, VisualisationType.MapCone, Dataset.Dataset4, 32, 33),
			Question.createLarger(7, VisualisationType.InPlaceBars, Dataset.Dataset4, 6, 7),
			Question.createLarger(8, VisualisationType.BarCone, Dataset.Dataset4, 12, 13),
			// 4
			Question.createLarger(9, VisualisationType.InPlaceBars, Dataset.Dataset4, 2, 3),
			Question.createLarger(10, VisualisationType.BarCone, Dataset.Dataset4, 20, 21),
			Question.createLarger(11, VisualisationType.MapCone, Dataset.Dataset4, 30, 31),
			// 5
			Question.createLarger(12, VisualisationType.BarCone, Dataset.Dataset4, 14, 15),
			Question.createLarger(13, VisualisationType.MapCone, Dataset.Dataset4, 34, 35),
			Question.createLarger(14, VisualisationType.InPlaceBars, Dataset.Dataset4, 4, 5),
			// 6
			Question.createLarger(15, VisualisationType.MapCone, Dataset.Dataset4, 24, 25),
			Question.createLarger(16, VisualisationType.InPlaceBars, Dataset.Dataset4, 8, 9),
			Question.createLarger(17, VisualisationType.BarCone, Dataset.Dataset4, 18, 19),
		});
	}

	void setUpDataset5()
	{
		string group1Dataset5 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset5.ToString();
		datasetQuestions.Add(group1Dataset5, new List<Question>() { 
			// 1
			Question.createCloser(0, VisualisationType.InPlaceBars, Dataset.Dataset5, 14, 15),
			Question.createCloser(1, VisualisationType.BarCone, Dataset.Dataset5, 20, 21),
			Question.createCloser(2, VisualisationType.MapCone, Dataset.Dataset5, 34, 35),
			// 2
			Question.createCloser(3, VisualisationType.BarCone, Dataset.Dataset5, 26, 27),
			Question.createCloser(4, VisualisationType.MapCone, Dataset.Dataset5, 44, 45),
			Question.createCloser(5, VisualisationType.InPlaceBars, Dataset.Dataset5, 2, 3),
			// 3
			Question.createCloser(6, VisualisationType.MapCone, Dataset.Dataset5, 36, 37),
			Question.createCloser(7, VisualisationType.InPlaceBars, Dataset.Dataset5, 10, 11),
			Question.createCloser(8, VisualisationType.BarCone, Dataset.Dataset5, 28, 29),
			// 4
			Question.createCloser(9, VisualisationType.InPlaceBars, Dataset.Dataset5, 4, 5),
			Question.createCloser(10, VisualisationType.BarCone, Dataset.Dataset5, 18, 19),
			Question.createCloser(11, VisualisationType.MapCone, Dataset.Dataset5, 40, 41),
			// 5
			Question.createCloser(12, VisualisationType.BarCone, Dataset.Dataset5, 24, 25),
			Question.createCloser(13, VisualisationType.MapCone, Dataset.Dataset5, 38, 39),
			Question.createCloser(14, VisualisationType.InPlaceBars, Dataset.Dataset5, 0, 1),
			// 6
			Question.createCloser(15, VisualisationType.MapCone, Dataset.Dataset5, 42, 43),
			Question.createCloser(16, VisualisationType.InPlaceBars, Dataset.Dataset5, 12, 13),
			Question.createCloser(17, VisualisationType.BarCone, Dataset.Dataset5, 22, 23),
			// 7
			Question.createCloser(18, VisualisationType.InPlaceBars, Dataset.Dataset5, 6, 7),
			Question.createCloser(19, VisualisationType.BarCone, Dataset.Dataset5, 16, 17),
			Question.createCloser(20, VisualisationType.MapCone, Dataset.Dataset5, 46, 47),
			// 8
			Question.createCloser(21, VisualisationType.BarCone, Dataset.Dataset5, 30, 31),
			Question.createCloser(22, VisualisationType.MapCone, Dataset.Dataset5, 32, 33),
			Question.createCloser(23, VisualisationType.InPlaceBars, Dataset.Dataset5, 8, 9),
		});
	}

	void setUpDataset6()
	{
		string group1Dataset6 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset6.ToString();
		datasetQuestions.Add(group1Dataset6, new List<Question>() { 
			// 1
			Question.createCloser(0, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 14, 15),
			Question.createCloser(1, VisualisationType.BarCone, 		Dataset.Dataset6, 20, 21),
			Question.createCloser(2, VisualisationType.MapCone, 		Dataset.Dataset6, 34, 35),
			// 2
			Question.createCloser(3, VisualisationType.BarCone, 		Dataset.Dataset6, 26, 27),
			Question.createCloser(4, VisualisationType.MapCone, 		Dataset.Dataset6, 44, 45),
			Question.createCloser(5, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 2, 3),
			// 3
			Question.createCloser(6, VisualisationType.MapCone, 		Dataset.Dataset6, 36, 37),
			Question.createCloser(7, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 10, 11),
			Question.createCloser(8, VisualisationType.BarCone, 		Dataset.Dataset6, 28, 29),
			// 4
			Question.createCloser(9, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 4, 5),
			Question.createCloser(10, VisualisationType.BarCone, 		Dataset.Dataset6, 18, 19),
			Question.createCloser(11, VisualisationType.MapCone, 		Dataset.Dataset6, 40, 41),
			// 5
			Question.createCloser(12, VisualisationType.BarCone, 		Dataset.Dataset6, 24, 25),
			Question.createCloser(13, VisualisationType.MapCone, 		Dataset.Dataset6, 38, 39),
			Question.createCloser(14, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 0, 1),
			// 6
			Question.createCloser(15, VisualisationType.MapCone, 		Dataset.Dataset6, 42, 43),
			Question.createCloser(16, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 12, 13),
			Question.createCloser(17, VisualisationType.BarCone, 		Dataset.Dataset6, 22, 23),
			// 7
			Question.createCloser(18, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 6, 7),
			Question.createCloser(19, VisualisationType.BarCone, 		Dataset.Dataset6, 16, 17),
			Question.createCloser(20, VisualisationType.MapCone, 		Dataset.Dataset6, 46, 47),
			// 8
			Question.createCloser(21, VisualisationType.BarCone, 		Dataset.Dataset6, 30, 31),
			Question.createCloser(22, VisualisationType.MapCone, 		Dataset.Dataset6, 32, 33),
			Question.createCloser(23, VisualisationType.InPlaceBars, 	Dataset.Dataset6, 8, 9),
		});
	}

	void setUpDataset7()
	{
		string group1Dataset7 = ParticipantGroup.Group1.ToString() + "_" + Dataset.Dataset7.ToString();
		datasetQuestions.Add(group1Dataset7, new List<Question>() { 
			// 1
			Question.createCloser(0, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 14, 15),
			Question.createCloser(1, VisualisationType.BarCone, 		Dataset.Dataset7, 20, 21),
			Question.createCloser(2, VisualisationType.MapCone, 		Dataset.Dataset7, 34, 35),
			// 2
			Question.createCloser(3, VisualisationType.BarCone, 		Dataset.Dataset7, 26, 27),
			Question.createCloser(4, VisualisationType.MapCone, 		Dataset.Dataset7, 44, 45),
			Question.createCloser(5, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 2, 3),
			// 3
			Question.createCloser(6, VisualisationType.MapCone, 		Dataset.Dataset7, 36, 37),
			Question.createCloser(7, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 10, 11),
			Question.createCloser(8, VisualisationType.BarCone, 		Dataset.Dataset7, 28, 29),
			// 4
			Question.createCloser(9, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 4, 5),
			Question.createCloser(10, VisualisationType.BarCone, 		Dataset.Dataset7, 18, 19),
			Question.createCloser(11, VisualisationType.MapCone, 		Dataset.Dataset7, 40, 41),
			// 5
			Question.createCloser(12, VisualisationType.BarCone, 		Dataset.Dataset7, 24, 25),
			Question.createCloser(13, VisualisationType.MapCone, 		Dataset.Dataset7, 38, 39),
			Question.createCloser(14, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 0, 1),
			// 6
			Question.createCloser(15, VisualisationType.MapCone, 		Dataset.Dataset7, 42, 43),
			Question.createCloser(16, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 12, 13),
			Question.createCloser(17, VisualisationType.BarCone, 		Dataset.Dataset7, 22, 23),
			// 7
			Question.createCloser(18, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 6, 7),
			Question.createCloser(19, VisualisationType.BarCone, 		Dataset.Dataset7, 16, 17),
			Question.createCloser(20, VisualisationType.MapCone, 		Dataset.Dataset7, 46, 47),
			// 8
			Question.createCloser(21, VisualisationType.BarCone, 		Dataset.Dataset7, 30, 31),
			Question.createCloser(22, VisualisationType.MapCone, 		Dataset.Dataset7, 32, 33),
			Question.createCloser(23, VisualisationType.InPlaceBars, 	Dataset.Dataset7, 8, 9),
		});
	}

	string getDatasetKey(Dataset dataset, ParticipantGroup group)
	{
		return group.ToString() + "_" + dataset.ToString();
	}
}
