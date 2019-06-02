using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.MeshGeneration.Data;
using System.Text;
using System;

public enum MeshSelection
{
    Cube,
    Cylinder,
    Quad
}

public enum VisualisationType
{
    None,
    InPlaceBars,
    ProjectingCone,
    MapCone,
    BarCone,
}

public enum MapSelection
{
    Map1,
    Map2
}

public class LoadDataSet : MonoBehaviour {
    public AbstractMap _map;

    private String datasetDir = "Datasets";

    private String datasetFile = "vic_wateruse_2008_2009.csv";
    private String trainingDatasetFile = "data_20190515141314.csv";
    private String testDatasetFile = "dataset_est.csv";

    [SerializeField]
    public GameObject _player = null;

    public VisualisationType _visualisationType = VisualisationType.InPlaceBars;

    [Header("In-place Bars")]
    public Transform _barsContainer;

    public Mesh _cubeMesh;
    public Mesh _cylinderMesh;
    public Mesh _quadMesh;
    public MeshSelection _meshSelectionType;

    public GameObject _locationMarkerBar;

    public delegate void OnDataLoaded();
    public static event OnDataLoaded onDataLoaded;

    [SerializeField]
    private GameObject _framedBar;
    private List<GameObject> _bars = new List<GameObject>();
    private float _maxBarHeight = 50.0f;
    private float _barHeightBuffer = 1.2f;
    private Mesh _meshSelection;
    private Dictionary<MeshSelection, Mesh> _meshes;

    [Header("Interaction")]
    public ControllerBehavior _leftController;
    public ControllerBehavior _rightController;

    private DataPointsManager dataPoinstsManager;

    private bool mapFullyLoaded = false;

    private float mapMaxX = -Mathf.Infinity;
    private float mapMaxY = -Mathf.Infinity;

    private float mapMinX = Mathf.Infinity;
    private float mapMinY = Mathf.Infinity;

    public Dataset _currentDataset = Dataset.None;
    public ParticipantGroup _participantGroup = ParticipantGroup.Group1;
    public int _userId;

    public int _startQuestionIdx = 0;

    public bool needToReloadBars = false;

    private AudioSource _audioData;

    private void Awake()
    {
        //_map.OnInitialized += placeBarChart;

        _audioData = GetComponent<AudioSource>();

        if (_meshSelectionType == MeshSelection.Cube)
        {
            _meshSelection = _cubeMesh;
        } else if (_meshSelectionType == MeshSelection.Cylinder) 
        {
            _meshSelection = _cylinderMesh;
        } else if (_meshSelectionType == MeshSelection.Quad)
        {
            _meshSelection = _quadMesh;
        }
    }

    void addBlankBars()
    {
        var points = DataPointsManager.Instance.mapDataPoints;
        foreach(MapDataPoint point in points)
        {
            GameObject bar = Instantiate(_locationMarkerBar, point.WorldPosition, Quaternion.identity);
            bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + point.Name;

            LocationMarkerBehavior barBehavior = bar.GetComponent<LocationMarkerBehavior>();
            barBehavior.mapDataPoint = point;
            _bars.Add(bar);
        }
    }

    void addInPlaceBars()
    {
        _meshes = new Dictionary<MeshSelection, Mesh>();
        _meshes.Add(MeshSelection.Cube, _cubeMesh);
        _meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        _meshes.Add(MeshSelection.Quad, _quadMesh);

        var points = DataPointsManager.Instance.mapDataPoints;
        var maxValue = DataPointsManager.Instance.maxValue;
        foreach(MapDataPoint point in points)
        {
            GameObject bar = Instantiate(_framedBar, point.WorldPosition, Quaternion.identity);
            bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + point.Name;

            FramedBarData barDataComponent = bar.GetComponent<FramedBarData>();
            barDataComponent.mapDataPoint = point;
            barDataComponent.Value = point.Value;
            barDataComponent.LatLong = point.GeoPosition;
            barDataComponent.Elevation = _map.QueryElevationInUnityUnitsAt(point.GeoPosition);
            barDataComponent.MeshType = _meshSelectionType;

            _bars.Add(bar);
        }

        foreach (GameObject bar in _bars)
        {
            FramedBarData barDataComponent = bar.GetComponent<FramedBarData>();
            barDataComponent.MaxValue = maxValue;
            barDataComponent.updateBars();
            //barDataComponent.shear();
        }
    }

    string normalisedTextData(string data)
    {
        var builder = new StringBuilder(data);
        builder.Replace("\"", "");
        builder.Replace(@"\u00A0", " ");

        return builder.ToString().Trim();
    }

    void Update()
    {
        if (!mapFullyLoaded)
        {
            Dictionary<Mapbox.Map.UnwrappedTileId, UnityTile> dict = _map.MapVisualizer.ActiveTiles;
            Debug.Log("Tile count: " + dict.Count);
            readSizeOfMap();
            loadTestDataset(_currentDataset);
            loadMeshes();
            mapFullyLoaded = true;
        }

        switch (StudyPlot.Instance.state)
        {
            case PlotState.NotStarted:
                StudyPlot.Instance.setStartQuestionIdx(_startQuestionIdx);
                var firstQuestion = StudyPlot.Instance.startPlot();
                handleQuestion(firstQuestion);
                break;
            case PlotState.ChangeDataset:
                needToReloadBars = true;
                _startQuestionIdx = 0;
                _currentDataset = StudyPlot.Instance.getNextDataset();
                loadTestDataset(_currentDataset);
                StudyPlot.Instance.state = PlotState.NotStarted;
                break;
            case PlotState.OnDoingQuestion:
                break;
            case PlotState.OnCompletedQuestion:
                _audioData.Play(0);
                completeQuestion(StudyPlot.Instance.currentQuestion());
                var nextQuestion = StudyPlot.Instance.nextQuestion();
                handleQuestion(nextQuestion);
                break;
            case PlotState.OnFinished:
                Debug.Log("Finish....");
                quitGame();
                break;
            default:
                break;
        }
    }

    void handleQuestion(Question question)
    {
        if (question == null)
        {
            return;
        }

        if (needToReloadBars)
        {
            destroyInPlaceBars();
            addInPlaceBars();
            needToReloadBars = false;
        }

        DataPointsManager.Instance.resetPosing();

        bool changed = (_visualisationType != question.visualisationType);

        if (changed)
        {
            _visualisationType = question.visualisationType;

            var barCone = _player.GetComponentInChildren<ConeRenderer>();
            var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();

            switch (_visualisationType)
            {
                case VisualisationType.InPlaceBars:
                    //destroyInPlaceBars();
                    //addInPlaceBars();
                    barCone.clearData();
                    mapCone.clearData();
                    _leftController._attachedCone = null;
                    _rightController._attachedCone = null;
                    break;
                case VisualisationType.BarCone:
                    //destroyInPlaceBars();
                    //addBlankBars();
                    _leftController._attachedCone = barCone;
                    _rightController._attachedCone = barCone;
                    barCone.initializeWithData();
                    mapCone.clearData();
                    break;
                case VisualisationType.MapCone:
                    //destroyInPlaceBars();
                    barCone.clearData();
                    //addBlankBars();
                    _leftController._attachedCone = mapCone;
                    _rightController._attachedCone = mapCone;
                    mapCone.initializeWithData();
                    break;
            }
        }

        _leftController.questionTask = question.task;
        _rightController.questionTask = question.task;

        switch(question.task)
        {
            case Task.EstimateSinglePoint:
                handleEstimateQuestion(question);
                break;
            case Task.PickLargerDataPoint:
                handleOptionQuestion(question);
                break;
            case Task.PickCloserDataPoint:
                handleOptionQuestion(question);
                break;
            default:
                break;
        }
    }

    void completeQuestion(Question question)
    {
        if (question == null)
        {
            return;
        }
        switch(question.task)
        {
            case Task.EstimateSinglePoint:
                question.getDataPoint1().completeQuestionOption1();
                break;
            case Task.PickLargerDataPoint:
            case Task.PickCloserDataPoint:
                question.getDataPoint1().completeQuestionOption1();
                question.getDataPoint2().completeQuestionOption2();
                break;
            default:
                break;
        }
    }

    void handleEstimateQuestion(Question question)
    {
        question.getDataPoint1().showQuestionOption1();
    }

    void handleOptionQuestion(Question question)
    {
        question.getDataPoint1().showQuestionOption1();
        question.getDataPoint2().showQuestionOption2();
    }

    void readSizeOfMap() {
        // Convert to the furthest
        // find the smallest/largest x and y

        UnityTile[] tiles = GetComponentsInChildren<UnityTile>(true);
        Debug.Log("Number of tiles: " + tiles.Length);
        for(int i=0; i < tiles.Length; i++) {
            UnityTile tile = tiles[i];
            float x = tile.transform.position.x;
            float y = tile.transform.position.z;
            if (x > mapMaxX)
                mapMaxX = x;

            if (x < mapMinX)
                mapMinX = x;

            if (y > mapMaxY)
                mapMaxY = y;

            if (y < mapMinY)
                mapMinY = y;
        }

        float tileLength = 50;
        mapMaxX += tileLength;
        mapMaxY += tileLength;
        mapMinX -= tileLength;
        mapMinY -= tileLength;

        
        DataPointsManager.Instance.Map = _map;
        DataPointsManager.Instance.MapMaxX = mapMaxX;
        DataPointsManager.Instance.MapMaxY = mapMaxY;
        DataPointsManager.Instance.MapMinX = mapMinX;
        DataPointsManager.Instance.MapMinY = mapMinY;

        Debug.Log("Max X: " + mapMaxX);
        Debug.Log("Max Y: " + mapMaxY);
        Debug.Log("Min X: " + mapMinX);
        Debug.Log("Min Y: " + mapMinY);
    }

    void loadTestDataset(Dataset dataset) 
    {
        StudyPlot.Instance.setUserId(_userId);
        StudyPlot.Instance.setDataset(dataset, _participantGroup);

        string data = System.IO.File.ReadAllText(StudyPlot.Instance.getDatasetFile());
        string[] lines = data.Split('\n');
        List<MapDataPoint> dataPoints = new List<MapDataPoint>();

        float maxValue = 1;

        DataPointsManager.Instance.mapDataPoints.Clear();
        // Skip the header
        for (int i = 1; i < lines.Length; i++)
        {
            string[] lineData = lines[i].Split(',');
            
            if (lineData.Length < 4) continue;

            MapDataPoint point = new MapDataPoint();
            point.Name = "Point " + lineData[0];
            point.Value = float.Parse(lineData[3]);
            var rawX = float.Parse(lineData[1]);
            var rawY = float.Parse(lineData[2]);

            Debug.Log("Point " + point.Name + ", X: " +  rawX + ", Y: " + rawY);

            point.RawPosition = new Vector2(rawX, rawY);

            //Debug.Log("Pos " + i + " : " + point.WorldPosition.ToString());
            DataPointsManager.Instance.mapDataPoints.Add(point);
        }

        needToReloadBars = true;

        Debug.Log("Dataset: " + dataset +
        " - " + StudyPlot.Instance._questions.Count + " questions" +
        " - " + DataPointsManager.Instance.mapDataPoints.Count + " datapoints");

        DataPointsManager.Instance.maxValue = maxValue;
    }

    void loadMeshes()
    {
        _meshes = new Dictionary<MeshSelection, Mesh>();
        _meshes.Add(MeshSelection.Cube, _cubeMesh);
        _meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        _meshes.Add(MeshSelection.Quad, _quadMesh);

        var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();
        mapCone._meshes = _meshes;
        mapCone._meshSelectionType = _meshSelectionType;
    }

    void destroyInPlaceBars()
    {
        //_barsContainer.DetachChildren();
		foreach(var bar in _bars)
			Destroy(bar);

		_bars.Clear();
    }

    void quitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}