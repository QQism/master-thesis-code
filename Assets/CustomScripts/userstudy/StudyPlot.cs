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
		_currentResponse = new UserResponse(userId, nextQuestion);
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
		_currentResponse = new UserResponse(userId, nextQuestion);
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

	public void setUserId(int userId)
	{
		this.userId = userId;
	}

	void setUpQuestions()
	{
		datasetFiles = new Dictionary<Dataset, string>();

		datasetFiles.Add(Dataset.Task1TrainingDataset, "dataset_est_training.csv");
		datasetFiles.Add(Dataset.Task2TrainingDataset, "dataset_higher_training.csv");
		datasetFiles.Add(Dataset.Task3TrainingDataset, "dataset_closer_training.csv");

		datasetFiles.Add(Dataset.Dataset1, "dataset_est_latest.csv");

		datasetFiles.Add(Dataset.Dataset2, "dataset_higher1_latest.csv");
		//datasetFiles.Add(Dataset.Dataset3, "dataset_higher2_latest.csv");
		//datasetFiles.Add(Dataset.Dataset4, "dataset_higher3_latest.csv");

		//datasetFiles.Add(Dataset.Dataset5, "dataset_closer1_latest.csv");
		datasetFiles.Add(Dataset.Dataset5, "dataset_closer1_latest_easier.csv");
		//datasetFiles.Add(Dataset.Dataset6, "dataset_closer2_latest.csv");
		//datasetFiles.Add(Dataset.Dataset7, "dataset_closer3_latest.csv");

		datasetQuestions = new Dictionary<string, List<Question>>();

		setUpTask1TrainingDataset(Dataset.Task1TrainingDataset);
		setUpTask2TrainingDataset(Dataset.Task2TrainingDataset);
		setUpTask3TrainingDataset(Dataset.Task3TrainingDataset);

		setUpTask1Dataset(Dataset.Dataset1);
		setUpTask2Dataset(Dataset.Dataset2);
		setUpTask3Dataset(Dataset.Dataset5);
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
                //nextDataset = Dataset.Dataset3;
                break;		
            case Dataset.Dataset5:
                //nextDataset = Dataset.Dataset6;
                break;
            case Dataset.Dataset1:
			default:
                break;
        }

		return nextDataset;
	}

	void setUpTask1Dataset(Dataset dataset)
	{
		int i = 0;
		// Group 1 - V1-V2-V3
		string group1Key = getDatasetKey(dataset, ParticipantGroup.Group1);
        datasetQuestions.Add(group1Key, new List<Question>() {
            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(i++, "V1R2Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(i++, "V1R2Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 3),

            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(i++, "V2R2Re1", VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(i++, "V2R2Re2", VisualisationType.BarCone, Dataset.Dataset1, 7),

            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(i++, "V3R2Re1", VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(i++, "V3R2Re2", VisualisationType.MapCone, Dataset.Dataset1, 11),
        });

		checkDuplicateQuestions(datasetQuestions[group1Key]);

		i = 0;
		// Group 2 - V2-V3-V1
		string group2Key = getDatasetKey(dataset, ParticipantGroup.Group2);
        datasetQuestions.Add(group2Key, new List<Question>() {
            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(i++, "V2R2Re1", VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(i++, "V2R2Re2", VisualisationType.BarCone, Dataset.Dataset1, 7),

            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(i++, "V3R2Re1", VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(i++, "V3R2Re2", VisualisationType.MapCone, Dataset.Dataset1, 11),

            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(i++, "V1R2Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(i++, "V1R2Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 3),
        });

		checkDuplicateQuestions(datasetQuestions[group2Key]);

		// Group 3 - V3-V1-V2
		string group3Key = getDatasetKey(dataset, ParticipantGroup.Group3);
        datasetQuestions.Add(group3Key, new List<Question>() {
            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, Dataset.Dataset1, 8),
            Question.createEstimate(i++, "V3R2Re1", VisualisationType.MapCone, Dataset.Dataset1, 10),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, Dataset.Dataset1, 9),
            Question.createEstimate(i++, "V3R2Re2", VisualisationType.MapCone, Dataset.Dataset1, 11),

            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 0),
            Question.createEstimate(i++, "V1R2Re1", VisualisationType.InPlaceBars, Dataset.Dataset1, 2),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 1),
            Question.createEstimate(i++, "V1R2Re2", VisualisationType.InPlaceBars, Dataset.Dataset1, 3),

            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, Dataset.Dataset1, 4),
            Question.createEstimate(i++, "V2R2Re1", VisualisationType.BarCone, Dataset.Dataset1, 6),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, Dataset.Dataset1, 5),
            Question.createEstimate(i++, "V2R2Re2", VisualisationType.BarCone, Dataset.Dataset1, 7),
        });

		checkDuplicateQuestions(datasetQuestions[group3Key]);
	}

	void setUpTask2Dataset(Dataset dataset)
	{
		int i = 0;
		// Group 1 - V1-V2-V3
		string group1Key = getDatasetKey(dataset, ParticipantGroup.Group1);
		datasetQuestions.Add(group1Key, new List<Question>() {
			Question.createLarger(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 4, 5),
			Question.createLarger(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 8, 9),
			Question.createLarger(i++, "V1R3F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 20, 21),
			Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 0, 1),

			Question.createLarger(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 12, 13),
			Question.createLarger(i++, "V1R3F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 16, 17),
			Question.createLarger(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 6, 7),
			Question.createLarger(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 10, 11),

			Question.createLarger(i++, "V1R3F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 22, 23),
			Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 2, 3),
			Question.createLarger(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 14, 15),
			Question.createLarger(i++, "V1R3F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 18, 19),

			Question.createLarger(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 28, 29),
			Question.createLarger(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 32, 33),
			Question.createLarger(i++, "V2R3F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 44, 45),
			Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 24, 25),

			Question.createLarger(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 36, 37),
			Question.createLarger(i++, "V2R3F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 40, 41),
			Question.createLarger(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 30, 31),
			Question.createLarger(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 34, 35),

			Question.createLarger(i++, "V2R3F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 46, 47),
			Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 26, 27),
			Question.createLarger(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 38, 39),
			Question.createLarger(i++, "V2R3F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 42, 43),

			Question.createLarger(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 52, 53),
			Question.createLarger(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 56, 57),
			Question.createLarger(i++, "V3R3F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 68, 69),
			Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 48, 49),

			Question.createLarger(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 60, 61),
			Question.createLarger(i++, "V3R3F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 64, 65),
			Question.createLarger(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 54, 55),
			Question.createLarger(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 58, 59),

			Question.createLarger(i++, "V3R3F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 70, 71),
			Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 50, 51),
			Question.createLarger(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 62, 63),
			Question.createLarger(i++, "V3R3F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 66, 67),
		});

		checkDuplicateQuestions(datasetQuestions[group1Key]);

		i = 0;
		// Group 2 - V2-V3-V1
		string group2Key = getDatasetKey(dataset, ParticipantGroup.Group2);
		datasetQuestions.Add(group2Key, new List<Question>() {
			Question.createLarger(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 28, 29),
			Question.createLarger(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 32, 33),
			Question.createLarger(i++, "V2R3F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 44, 45),
			Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 24, 25),

			Question.createLarger(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 36, 37),
			Question.createLarger(i++, "V2R3F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 40, 41),
			Question.createLarger(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 30, 31),
			Question.createLarger(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 34, 35),

			Question.createLarger(i++, "V2R3F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 46, 47),
			Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 26, 27),
			Question.createLarger(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 38, 39),
			Question.createLarger(i++, "V2R3F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 42, 43),

			Question.createLarger(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 52, 53),
			Question.createLarger(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 56, 57),
			Question.createLarger(i++, "V3R3F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 68, 69),
			Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 48, 49),

			Question.createLarger(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 60, 61),
			Question.createLarger(i++, "V3R3F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 64, 65),
			Question.createLarger(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 54, 55),
			Question.createLarger(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 58, 59),

			Question.createLarger(i++, "V3R3F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 70, 71),
			Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 50, 51),
			Question.createLarger(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 62, 63),
			Question.createLarger(i++, "V3R3F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 66, 67),

			Question.createLarger(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 4, 5),
			Question.createLarger(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 8, 9),
			Question.createLarger(i++, "V1R3F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 20, 21),
			Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 0, 1),

			Question.createLarger(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 12, 13),
			Question.createLarger(i++, "V1R3F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 16, 17),
			Question.createLarger(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 6, 7),
			Question.createLarger(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 10, 11),

			Question.createLarger(i++, "V1R3F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 22, 23),
			Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 2, 3),
			Question.createLarger(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 14, 15),
			Question.createLarger(i++, "V1R3F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 18, 19),
		});

		checkDuplicateQuestions(datasetQuestions[group2Key]);

		i = 0;
		// Group 3 - V3-V1-V2
		string group3Key = getDatasetKey(dataset, ParticipantGroup.Group3);
		datasetQuestions.Add(group3Key, new List<Question>() {
			Question.createLarger(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 52, 53),
			Question.createLarger(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 56, 57),
			Question.createLarger(i++, "V3R3F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 68, 69),
			Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 48, 49),

			Question.createLarger(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset2, 60, 61),
			Question.createLarger(i++, "V3R3F1Re1", VisualisationType.MapCone, Dataset.Dataset2, 64, 65),
			Question.createLarger(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 54, 55),
			Question.createLarger(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 58, 59),

			Question.createLarger(i++, "V3R3F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 70, 71),
			Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 50, 51),
			Question.createLarger(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset2, 62, 63),
			Question.createLarger(i++, "V3R3F1Re2", VisualisationType.MapCone, Dataset.Dataset2, 66, 67),

			Question.createLarger(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 4, 5),
			Question.createLarger(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 8, 9),
			Question.createLarger(i++, "V1R3F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 20, 21),
			Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 0, 1),

			Question.createLarger(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 12, 13),
			Question.createLarger(i++, "V1R3F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset2, 16, 17),
			Question.createLarger(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 6, 7),
			Question.createLarger(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 10, 11),

			Question.createLarger(i++, "V1R3F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 22, 23),
			Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 2, 3),
			Question.createLarger(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 14, 15),
			Question.createLarger(i++, "V1R3F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset2, 18, 19),

			Question.createLarger(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 28, 29),
			Question.createLarger(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 32, 33),
			Question.createLarger(i++, "V2R3F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 44, 45),
			Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 24, 25),

			Question.createLarger(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset2, 36, 37),
			Question.createLarger(i++, "V2R3F1Re1", VisualisationType.BarCone, Dataset.Dataset2, 40, 41),
			Question.createLarger(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 30, 31),
			Question.createLarger(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 34, 35),

			Question.createLarger(i++, "V2R3F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 46, 47),
			Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 26, 27),
			Question.createLarger(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset2, 38, 39),
			Question.createLarger(i++, "V2R3F1Re2", VisualisationType.BarCone, Dataset.Dataset2, 42, 43),
		});

		checkDuplicateQuestions(datasetQuestions[group3Key]);
	}

	void setUpTask3Dataset(Dataset dataset)
	{
		int i = 0;
		// Group 1 - V1-V2-V3
		string group1Key = getDatasetKey(dataset, ParticipantGroup.Group1);
		datasetQuestions.Add(group1Key, new List<Question>() { 
			Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 0, 1),
			Question.createCloser(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 10, 11),
			Question.createCloser(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 2, 3),
			Question.createCloser(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 8, 9),

			Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 4, 5),
			Question.createCloser(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 14, 15),
			Question.createCloser(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 6, 7),
			Question.createCloser(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 12, 13),

			Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 16, 17),
			Question.createCloser(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 26, 27),
			Question.createCloser(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 18, 19),
			Question.createCloser(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 24, 25),

			Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 20, 21),
			Question.createCloser(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 30, 31),
			Question.createCloser(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 22, 23),
			Question.createCloser(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 28, 29),

			Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 32, 33),
			Question.createCloser(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 42, 43),
			Question.createCloser(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 34, 35),
			Question.createCloser(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 40, 41),

			Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 36, 37),
			Question.createCloser(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 46, 47),
			Question.createCloser(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 38, 39),
			Question.createCloser(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 44, 45),
		});

		checkDuplicateQuestions(datasetQuestions[group1Key]);

		i = 0;
		// Group 2 - V2-V3-V1
		string group2Key = getDatasetKey(dataset, ParticipantGroup.Group2);
		datasetQuestions.Add(group2Key, new List<Question>() { 
			Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 16, 17),
			Question.createCloser(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 26, 27),
			Question.createCloser(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 18, 19),
			Question.createCloser(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 24, 25),

			Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 20, 21),
			Question.createCloser(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 30, 31),
			Question.createCloser(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 22, 23),
			Question.createCloser(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 28, 29),

			Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 32, 33),
			Question.createCloser(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 42, 43),
			Question.createCloser(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 34, 35),
			Question.createCloser(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 40, 41),

			Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 36, 37),
			Question.createCloser(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 46, 47),
			Question.createCloser(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 38, 39),
			Question.createCloser(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 44, 45),

			Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 0, 1),
			Question.createCloser(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 10, 11),
			Question.createCloser(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 2, 3),
			Question.createCloser(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 8, 9),

			Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 4, 5),
			Question.createCloser(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 14, 15),
			Question.createCloser(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 6, 7),
			Question.createCloser(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 12, 13),
		});

		checkDuplicateQuestions(datasetQuestions[group2Key]);

		i = 0;
		// Group 3 - V3-V1-V2
		string group3Key = getDatasetKey(dataset, ParticipantGroup.Group3);
		datasetQuestions.Add(group3Key, new List<Question>() { 
			Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 32, 33),
			Question.createCloser(i++, "V3R2F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 42, 43),
			Question.createCloser(i++, "V3R1F2Re1", VisualisationType.MapCone, Dataset.Dataset5, 34, 35),
			Question.createCloser(i++, "V3R2F1Re1", VisualisationType.MapCone, Dataset.Dataset5, 40, 41),

			Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 36, 37),
			Question.createCloser(i++, "V3R2F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 46, 47),
			Question.createCloser(i++, "V3R1F2Re2", VisualisationType.MapCone, Dataset.Dataset5, 38, 39),
			Question.createCloser(i++, "V3R2F1Re2", VisualisationType.MapCone, Dataset.Dataset5, 44, 45),

			Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 0, 1),
			Question.createCloser(i++, "V1R2F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 10, 11),
			Question.createCloser(i++, "V1R1F2Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 2, 3),
			Question.createCloser(i++, "V1R2F1Re1", VisualisationType.InPlaceBars, Dataset.Dataset5, 8, 9),

			Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 4, 5),
			Question.createCloser(i++, "V1R2F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 14, 15),
			Question.createCloser(i++, "V1R1F2Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 6, 7),
			Question.createCloser(i++, "V1R2F1Re2", VisualisationType.InPlaceBars, Dataset.Dataset5, 12, 13),

			Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 16, 17),
			Question.createCloser(i++, "V2R2F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 26, 27),
			Question.createCloser(i++, "V2R1F2Re1", VisualisationType.BarCone, Dataset.Dataset5, 18, 19),
			Question.createCloser(i++, "V2R2F1Re1", VisualisationType.BarCone, Dataset.Dataset5, 24, 25),

			Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 20, 21),
			Question.createCloser(i++, "V2R2F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 30, 31),
			Question.createCloser(i++, "V2R1F2Re2", VisualisationType.BarCone, Dataset.Dataset5, 22, 23),
			Question.createCloser(i++, "V2R2F1Re2", VisualisationType.BarCone, Dataset.Dataset5, 28, 29),
		});

		checkDuplicateQuestions(datasetQuestions[group3Key]);
	}

	void setUpTask1TrainingDataset(Dataset dataset)
	{
		int i = 0;
		// Group 1 - V1-V2-V3
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group1), new List<Question>()
        {
            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, dataset, 0),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, dataset, 1),

            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, dataset, 2),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, dataset, 3),

            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, dataset, 4),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, dataset, 5),
        });

		i = 0;
		// Group 2 - V2-V3-V1
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group2), new List<Question>()
        {
            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, dataset, 2),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, dataset, 3),

            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, dataset, 4),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, dataset, 5),

            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, dataset, 0),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, dataset, 1),
        });

		i = 0;
		// Group 3 - V3-V1-V2
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group3), new List<Question>()
        {
            Question.createEstimate(i++, "V3R1Re1", VisualisationType.MapCone, dataset, 4),
            Question.createEstimate(i++, "V3R1Re2", VisualisationType.MapCone, dataset, 5),

            Question.createEstimate(i++, "V1R1Re1", VisualisationType.InPlaceBars, dataset, 0),
            Question.createEstimate(i++, "V1R1Re2", VisualisationType.InPlaceBars, dataset, 1),

            Question.createEstimate(i++, "V2R1Re1", VisualisationType.BarCone, dataset, 2),
            Question.createEstimate(i++, "V2R1Re2", VisualisationType.BarCone, dataset, 3),
        });
	}

    void setUpTask2TrainingDataset(Dataset dataset)
    {
		int i = 0;
		// Group 1 - V1-V2-V3
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group1), new List<Question>()
        {
            Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createLarger(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),

            Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createLarger(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),

            Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createLarger(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),
        });

		i = 0;
		// Group 2 - V2-V3-V1
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group2), new List<Question>()
        {
            Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createLarger(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),

            Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createLarger(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),

            Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createLarger(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),
        });

		i = 0;
		// Group 3 - V3-V1-V2
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group3), new List<Question>()
        {
            Question.createLarger(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createLarger(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createLarger(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),

            Question.createLarger(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createLarger(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createLarger(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),

            Question.createLarger(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createLarger(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createLarger(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),
        });
    }

	void setUpTask3TrainingDataset(Dataset dataset)
	{
		int i = 0;
		// Group 1 - V1-V2-V3
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group1), new List<Question>()
        {
            Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createCloser(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),

            Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createCloser(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),

            Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createCloser(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),
        });

		i = 0;
		// Group 2 - V2-V3-V1
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group2), new List<Question>()
        {
            Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createCloser(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),

            Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createCloser(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),

            Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createCloser(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),
        });

		i = 0;
		// Group 3 - V3-V1-V2
        datasetQuestions.Add(getDatasetKey(dataset, ParticipantGroup.Group3), new List<Question>()
        {
            Question.createCloser(i++, "V3R1F1Re1", VisualisationType.MapCone, dataset, 12, 13),
            Question.createCloser(i++, "V3R1F1Re2", VisualisationType.MapCone, dataset, 14, 15),
            Question.createCloser(i++, "V3R1F1Re3", VisualisationType.MapCone, dataset, 16, 17),

            Question.createCloser(i++, "V1R1F1Re1", VisualisationType.InPlaceBars, dataset, 0, 1),
            Question.createCloser(i++, "V1R1F1Re2", VisualisationType.InPlaceBars, dataset, 2, 3),
            Question.createCloser(i++, "V1R1F1Re3", VisualisationType.InPlaceBars, dataset, 4, 5),

            Question.createCloser(i++, "V2R1F1Re1", VisualisationType.BarCone, dataset, 6, 7),
            Question.createCloser(i++, "V2R1F1Re2", VisualisationType.BarCone, dataset, 8, 9),
            Question.createCloser(i++, "V2R1F1Re3", VisualisationType.BarCone, dataset, 10, 11),
        });
	}

	string getDatasetKey(Dataset dataset, ParticipantGroup group)
	{
		return group.ToString() + "_" + dataset.ToString();
	}

	void checkDuplicateQuestions(List<Question> questions)
	{
		HashSet<String> questionCodes = new HashSet<String>();
		HashSet<int> questionDataPoinnt1Idx = new HashSet<int>();
		HashSet<int> questionDataPoinnt2Idx = new HashSet<int>();
		foreach (Question question in questions)
		{
			if (!questionCodes.Add(question.code))
			{
				Debug.Log(question.code);
				throw new Exception("Duplicate Question Code");
			}

			if (!questionDataPoinnt1Idx.Add(question.dataPoint1Idx))
			{
				Debug.Log(question.dataPoint1Idx);
				throw new Exception("Duplicate Question DataPoint 1 idx");
			}

            if (question.task != Task.EstimateSinglePoint)
            {
                if (!questionDataPoinnt2Idx.Add(question.dataPoint2Idx))
                {
                    Debug.Log(question.dataPoint2Idx);
                    throw new Exception("Duplicate Question DataPoint 2 idx");
                }
            }
		}
	}
}
