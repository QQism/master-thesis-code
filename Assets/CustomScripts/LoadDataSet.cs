using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Unity.Location;
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
    InPlaceBars,
    ProjectingCone,
    MapCone,
}

public enum ConeType {
    None,
    BarCone,
    MapCone,
}

public class LoadDataSet : MonoBehaviour {
    public AbstractMap _map;

    private String datasetDir = "Datasets";

    private String datasetFile = "vic_wateruse_2008_2009.csv";

    [SerializeField]
    public GameObject _player = null;

    public VisualisationType _visualisationType = VisualisationType.InPlaceBars;

    public ConeType _coneType = ConeType.MapCone;

    [Header("In-place Bars")]
    public Transform _barsContainer;

    public Mesh _cubeMesh;
    public Mesh _cylinderMesh;
    public Mesh _quadMesh;
    public MeshSelection _meshSelectionType;

    public delegate void OnDataLoaded();
    public static event OnDataLoaded onDataLoaded;

    [SerializeField]
    private GameObject _framedBar;
    private List<GameObject> _bars = new List<GameObject>();
    private float _maxBarHeight = 50.0f;
    private float _barHeightBuffer = 1.2f;
    private Mesh _meshSelection;

    private Dictionary<MeshSelection, Mesh> _meshes;


	// Use this for initialization
	void Start () { }

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
        string filePath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            datasetDir,
            datasetFile
        });

        Debug.Log("File path:" + filePath);

        string data = System.IO.File.ReadAllText(filePath);
        string[] lines = data.Split('\n');
        List<MapDataPoint> dataPoints = new List<MapDataPoint>();

        float maxValue = 0;
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

            if (maxValue < point.Value)
                maxValue = point.Value;
        }

        // Skip the header
        Debug.Log("Number of records: " + lines.Length.ToString());
        switch(_visualisationType)
        {
            case VisualisationType.InPlaceBars:
                addInPlaceBars(dataPoints, maxValue);
                break;
            case VisualisationType.ProjectingCone:
                break;
        }

        switch(_coneType)
        {
            case ConeType.BarCone:
                var barCone = _player.GetComponentInChildren<ConeRenderer>();
                barCone.initializeWithData(dataPoints, maxValue);
                break;
            case ConeType.MapCone:
                var mapCone = _player.GetComponentInChildren<ConeMapRenderer>();
                mapCone._meshes = _meshes;
                mapCone._meshSelectionType = _meshSelectionType;
                mapCone._barMaxValue = maxValue;
                mapCone.initializeWithData(dataPoints, maxValue);
                break;
        }
    }

    void addInPlaceBars(List<MapDataPoint> points, float maxValue)
    {
        _meshes = new Dictionary<MeshSelection, Mesh>();
        _meshes.Add(MeshSelection.Cube, _cubeMesh);
        _meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        _meshes.Add(MeshSelection.Quad, _quadMesh);

        foreach(MapDataPoint point in points)
        {
            //Debug.Log(name + "[Height]: " + _map.GeoToWorldPosition(position, true));
            //Debug.Log(name + "[No Height]: " + _map.GeoToWorldPosition(position, false));
            //Debug.Log(name + "[Default]: " + _map.GeoToWorldPosition(position));

            GameObject bar = Instantiate(_framedBar, point.WorldPosition, Quaternion.identity);
            bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + point.Name;

            FramedBarData barDataComponent = bar.GetComponent<FramedBarData>();
            barDataComponent.Value = point.Value;
            barDataComponent.LatLong = point.GeoPosition;
            barDataComponent.Elevation = _map.QueryElevationInUnityUnitsAt(point.GeoPosition);
            barDataComponent.AvailableMeshes = _meshes;
            barDataComponent.MeshType = _meshSelectionType;

            _bars.Add(bar);
        }

        foreach (GameObject bar in _bars)
        {
            FramedBarData barDataComponent = bar.GetComponent<FramedBarData>();
            barDataComponent.MaxValue = maxValue;
            barDataComponent.updateBars();
            barDataComponent.shear();
        }
    }

    string normalisedTextData(string data)
    {
        var builder = new StringBuilder(data);
        builder.Replace("\"", "");
        builder.Replace(@"\u00A0", " ");

        return builder.ToString().Trim();
    }
}
