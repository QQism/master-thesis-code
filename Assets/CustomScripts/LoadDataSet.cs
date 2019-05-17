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
    private String testDatasetFile = "data_20190515141314.csv";

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
    public ControllerBehavior _controller;

    private DataPointsManager dataPoinstsManager;

    private bool mapFullyLoaded = false;

    private float mapMaxX = -Mathf.Infinity;
    private float mapMaxY = -Mathf.Infinity;

    private float mapMinX = Mathf.Infinity;
    private float mapMinY = Mathf.Infinity;

    private void Awake()
    {
        //_map.OnInitialized += placeBarChart;

        _map.OnInitialized += loadCSVData;

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

    void loadCSVData()
    {
        float maxValue = 0;
        /*
        string filePath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            datasetDir,
            datasetFile
        });

        Debug.Log("File path:" + filePath);

        Debug.Log("User position: " + _map.WorldToGeoPosition(new Vector3(13.65f, 23.1f, -87.2f)));

        string data = System.IO.File.ReadAllText(filePath);
        string[] lines = data.Split('\n');
        List<MapDataPoint> dataPoints = new List<MapDataPoint>();

        string[] filters = { "Melbourne Cbd", "Caulfield North", "Clayton" };
        for (int i = 1; i < lines.Length; i++)
        {

            string[] lineData = lines[i].Split(',');
            if (lineData.Length < 5)
                continue;

            bool filtered = false;
            string stringData = normalisedTextData(lineData[5]);

            foreach (string filter in filters)
                if (filter.Equals(stringData, StringComparison.OrdinalIgnoreCase))
                {
                    filtered = false;
                    break;
                }

            if (filtered)
                continue;

            float latitude = float.Parse(lineData[3]);
            float longitude = float.Parse(lineData[4]);
            Vector2d position = new Vector2d(latitude, longitude);

            MapDataPoint point = new MapDataPoint();
            point.Name = stringData;
            point.GeoPosition = position;
            point.WorldPosition = _map.GeoToWorldPosition(position, true);
            //Debug.Log(point.WorldPosition.ToString("f4"));
            point.Value = float.Parse(lineData[1]);

            dataPoints.Add(point);
            DataPointsManager.Instance.mapDataPoints.Add(point);

            if (maxValue < point.Value)
                maxValue = point.Value;
        }

        Debug.Log("Number of records: " + lines.Length.ToString());
        

        DataPointsManager.Instance.maxValue = maxValue;
        _meshes = new Dictionary<MeshSelection, Mesh>();
        _meshes.Add(MeshSelection.Cube, _cubeMesh);
        _meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        _meshes.Add(MeshSelection.Quad, _quadMesh);
        // Skip the header
        switch(_visualisationType)
        {
            case VisualisationType.InPlaceBars:
                addInPlaceBars();
                break;
            case VisualisationType.BarCone:
                addBlankBars();
                var barCone = _player.GetComponentInChildren<ConeRenderer>();
                _controller._attachedCone = barCone;
                barCone.initializeWithData();
                break;
            case VisualisationType.MapCone:
                addBlankBars();
                var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();
                _controller._attachedCone = mapCone;
                mapCone._meshes = _meshes;
                mapCone._meshSelectionType = _meshSelectionType;
                mapCone._barMaxValue = maxValue;
                mapCone.initializeWithData();
                break;
        }
        */
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
        // Highlight options bars to choose
        /*
        var currentQuestion = StudyPlot.Instance.currentQuestion();
        if (currentQuestion.task == Task.EstimateSinglePoint)
        {
            var bar = _bars[currentQuestion.dataPoint1Idx];
        }
        else if (currentQuestion.task == Task.PickLargerDataPoint)
        { 
            var bar1 = _bars[currentQuestion.dataPoint1Idx];
            var bar2 = _bars[currentQuestion.dataPoint2Idx];
        }
        */
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
            loadTestDataset(testDatasetFile);
            mapFullyLoaded = true;
        }

        switch (StudyPlot.Instance.state)
        {
            case PlotState.NotStarted:
                StudyPlot.Instance.setStartQuestionIdx(0);
                var firstQuestion = StudyPlot.Instance.startPlot();
                handleQuestion(firstQuestion);
                break;
            case PlotState.OnDoingQuestion:
                break;
            case PlotState.OnCompletedQuestion:
                completeQuestion(StudyPlot.Instance.currentQuestion());
                var nextQuestion = StudyPlot.Instance.nextQuestion();
                handleQuestion(nextQuestion);
                break;
            case PlotState.OnFinished:
                Debug.Log("Finish....");
                Application.Quit();
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

        bool changed = (_visualisationType != question.visualisationType);

        if (changed)
        {
            _visualisationType = question.visualisationType;

            var barCone = _player.GetComponentInChildren<ConeRenderer>();
            var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();

            switch (_visualisationType)
            {
                case VisualisationType.InPlaceBars:
                    destroyInPlaceBars();
                    addInPlaceBars();
                    barCone.clearData();
                    mapCone.clearData();
                    _controller._attachedCone = null;
                    break;
                case VisualisationType.BarCone:
                    destroyInPlaceBars();
                    addBlankBars();
                    _controller._attachedCone = barCone;
                    barCone.initializeWithData();
                    mapCone.clearData();
                    break;
                case VisualisationType.MapCone:
                    destroyInPlaceBars();
                    barCone.clearData();
                    addBlankBars();
                    _controller._attachedCone = mapCone;
                    mapCone.initializeWithData();
                    break;
            }
        }

        switch(question.task)
        {
            case Task.EstimateSinglePoint:
                handleEstimateQuestion(question);
                break;
            case Task.PickLargerDataPoint:
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
                DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].completeQuestionOption1();
                break;
            case Task.PickLargerDataPoint:
                DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].completeQuestionOption1();
                DataPointsManager.Instance.mapDataPoints[question.dataPoint2Idx].completeQuestionOption2();
                break;
            default:
                break;
        }
    }

    void handleEstimateQuestion(Question question)
    {
        //TODO: instead of check _visualisationType, use question.visualisationType
        switch (_visualisationType)
        {
            case VisualisationType.InPlaceBars:
                DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].showQuestionOption1();
                // Highlight a single bar
                /* 
                var bar = _bars[question.dataPoint1Idx];
                var barData = bar.GetComponent<FramedBarData>();
                barData._selectedIdx = BarOption.Bar1;
                barData.updateBars();
                Debug.Log(bar);
                */

                break;
            case VisualisationType.BarCone:
                DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].showQuestionOption1();
                break;
            case VisualisationType.MapCone:
                DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].showQuestionOption1();
                break;
            default:
                break;
        }

        // Switch controller to Numeric answer mode
        var controllerBehavior = _controller.GetComponent<ControllerBehavior>();
        controllerBehavior._controllerMode = ControllerMode.NumericAnswerBoard;
    }

    void handleOptionQuestion(Question question)
    {
        // Highlight two bars
        DataPointsManager.Instance.mapDataPoints[question.dataPoint1Idx].showQuestionOption1();
        DataPointsManager.Instance.mapDataPoints[question.dataPoint2Idx].showQuestionOption2();
        /* 
        var bar1 = _bars[question.dataPoint1Idx];
        var barData1 = bar1.GetComponent<FramedBarData>();
        barData1._selectedIdx = BarOption.Bar1;
        barData1.updateBars();

        var bar2 = _bars[question.dataPoint2Idx];
        var barData2 = bar2.GetComponent<FramedBarData>();
        barData2._selectedIdx = BarOption.Bar2;
        barData2.updateBars();
        */

        // Switch controller to option answer mode
        var controllerBehavior = _controller.GetComponent<ControllerBehavior>();
        controllerBehavior._controllerMode = ControllerMode.OptionAnswerBoard;
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

        Debug.Log("Max X: " + mapMaxX);
        Debug.Log("Max Y: " + mapMaxY);
        Debug.Log("Min X: " + mapMinX);
        Debug.Log("Min Y: " + mapMinY);
    }

    void loadTestDataset(string filename) 
    {
        string filePath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            datasetDir,
            filename
        });

        string data = System.IO.File.ReadAllText(filePath);
        string[] lines = data.Split('\n');
        List<MapDataPoint> dataPoints = new List<MapDataPoint>();

        float maxValue = 1;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] lineData = lines[i].Split(',');
            
            if (lineData.Length < 3) continue;

            MapDataPoint point = new MapDataPoint();
            point.Name = "Point " + i;
            point.Value = float.Parse(lineData[2]);

            float x = ((float.Parse(lineData[0]) + 1) * (Mathf.Abs(mapMaxX) + Mathf.Abs(mapMinX)) / 2) - Mathf.Abs(mapMinX);
            float z = ((float.Parse(lineData[1]) + 1) * (Mathf.Abs(mapMaxY) + Mathf.Abs(mapMinY)) / 2) - Mathf.Abs(mapMinY);

            Vector2d latLong = _map.WorldToGeoPosition(new Vector3(x, 0, z));
            float height = _map.QueryElevationInUnityUnitsAt(latLong);
            point.GeoPosition = latLong;
            point.WorldPosition = new Vector3(x, height, z);
            Debug.Log("Pos " + i + " : " + point.WorldPosition.ToString());
            DataPointsManager.Instance.mapDataPoints.Add(point);
        }

        DataPointsManager.Instance.maxValue = maxValue;
        _meshes = new Dictionary<MeshSelection, Mesh>();
        _meshes.Add(MeshSelection.Cube, _cubeMesh);
        _meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        _meshes.Add(MeshSelection.Quad, _quadMesh);
        // Skip the header
        /*
        switch(_visualisationType)
        {
            case VisualisationType.InPlaceBars:
                addInPlaceBars();
                break;
            case VisualisationType.BarCone:
                addBlankBars();
                var barCone = _player.GetComponentInChildren<ConeRenderer>();
                _controller._attachedCone = barCone;
                barCone.initializeWithData();
                break;
            case VisualisationType.MapCone:
                addBlankBars();
                var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();
                _controller._attachedCone = mapCone;
                mapCone._meshes = _meshes;
                mapCone._meshSelectionType = _meshSelectionType;
                mapCone._barMaxValue = maxValue;
                mapCone.initializeWithData();
                break;
        }
        */
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
}