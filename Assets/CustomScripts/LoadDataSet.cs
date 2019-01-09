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
}

public class LoadDataSet : MonoBehaviour {
    public AbstractMap _map;
    public Camera _camera;
    private String datasetFile = "\\Datasets\\vic_wateruse_2008_2009.csv";


    public VisualisationType _visualisationType = VisualisationType.InPlaceBars;

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
        string data = System.IO.File.ReadAllText(Application.dataPath + datasetFile);
        string[] lines = data.Split('\n');

        // Skip the header
        Debug.Log("Number of records: " + lines.Length.ToString());
        switch(_visualisationType)
        {
            case VisualisationType.InPlaceBars:
                addInPlaceBars(lines);
                break;
            case VisualisationType.ProjectingCone:
                break;
        }
    }

    void addInPlaceBars(string[] lines)
    {
        string[] filters = { "Melbourne Cbd", "Caulfield North", "Clayton" };
        Dictionary<MeshSelection, Mesh> meshes = new Dictionary<MeshSelection, Mesh>();
        meshes.Add(MeshSelection.Cube, _cubeMesh);
        meshes.Add(MeshSelection.Cylinder, _cylinderMesh);
        meshes.Add(MeshSelection.Quad, _quadMesh);

        float maxValue = 0;
        for (int i = 1; i < lines.Length; i++)
        //for (int i=1; i < 2; i++)
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
            //Debug.Log(name + "[Height]: " + _map.GeoToWorldPosition(position, true));
            //Debug.Log(name + "[No Height]: " + _map.GeoToWorldPosition(position, false));
            //Debug.Log(name + "[Default]: " + _map.GeoToWorldPosition(position));

            GameObject bar = Instantiate(_framedBar, _map.GeoToWorldPosition(position, true), Quaternion.identity);
            bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + stringData;
            float amount = float.Parse(lineData[1]);

            FramedBarData barDataComponent = bar.GetComponent<FramedBarData>();
            barDataComponent.PlayerCamera = _camera;
            barDataComponent.Value = amount;
            barDataComponent.LatLong = position;
            barDataComponent.Elevation = _map.QueryElevationInUnityUnitsAt(position);
            barDataComponent.AvailableMeshes = meshes;
            barDataComponent.MeshType = _meshSelectionType;

            bar.GetComponent<RotationAdjustment>().PlayerCamera = _camera;

            if (maxValue < amount)
                maxValue = amount;

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
