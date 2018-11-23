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

public class TestScript : MonoBehaviour {
    public AbstractMap _map;
    public Camera _camera;
    private String datasetFile = "\\Datasets\\vic_wateruse_2008_2009.csv";
    public Transform _barsContainer;

    public delegate void OnDataLoaded();
    public static event OnDataLoaded onDataLoaded;

    [SerializeField]
    private GameObject _barChart;
    private List<GameObject> _bars = new List<GameObject>();
    private float _maxBarHeight = 50.0f;
    private float _barHeightBuffer = 1.2f;

	// Use this for initialization
	void Start () { }

    private void Awake()
    {
        //_map.OnInitialized += placeBarChart;
        _map.OnInitialized += loadCSVData;
    }

    void loadCSVData()
    {
        string data = System.IO.File.ReadAllText(Application.dataPath + datasetFile);
        string[] lines = data.Split('\n');

        // Skip the header
        string[] filters = { "Melbourne Cbd", "Caulfield North", "Clayton" };
        Debug.Log("Number of records: " + lines.Length.ToString());

        float maxValue = 0;
        for (int i=1; i < lines.Length; i++)
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
            GameObject bar = Instantiate(_barChart, _map.GeoToWorldPosition(position), Quaternion.identity);
            bar.transform.SetParent(_barsContainer, true);
            bar.transform.name = "Bar " + stringData;
            float amount = float.Parse(lineData[1]);
            bar.GetComponent<BarCustomData>().Value = amount;
            bar.GetComponent<BarCustomData>().LatLong = position; 

            if (maxValue < amount)
                maxValue = amount;

            _bars.Add(bar);
        }

        foreach (GameObject bar in _bars)
        {
            transformBarWithAmount(bar,
                bar.GetComponent<BarCustomData>().Value,
                _map.QueryElevationInUnityUnitsAt(bar.GetComponent<BarCustomData>().LatLong), maxValue);
        }
    }

    void transformBarWithAmount(GameObject bar, float amount, float elevation, float maxValue)
    {
        float scaledAmount = amount / (maxValue * _barHeightBuffer) * _maxBarHeight;
        bar.transform.localScale = new Vector3(bar.transform.localScale.x, scaledAmount, bar.transform.localScale.z);
        bar.transform.position += new Vector3(0, elevation + scaledAmount/2, 0);
    }

    void placeBarChart()
    {
        Vector2d[] positions = { new Vector2d(-37.81420, 144.96320),
        new Vector2d(-37.810, 144.970),
        new Vector2d(-37.830, 145.010),
        new Vector2d(-38.030, 145.310)};

        foreach (Vector2d pos in positions)
        {
            //Debug.Log("Before Transform:" + "  X:" + _cube.transform.position.x.ToString() + ", Y:" + _cube.transform.position.y.ToString() + ", Z:" + _cube.transform.position.z.ToString());
            //Debug.Log(_map.CenterLatitudeLongitude.ToString());
            //_cube.transform.position = _map.GeoToWorldPosition(positions[0]);
            //Debug.Log("After  Transform:" + "  X:" + _cube.transform.position.x.ToString() + ", Y:" + _cube.transform.position.y.ToString() + ", Z:" + _cube.transform.position.y.ToString());
            Instantiate(_barChart, _map.GeoToWorldPosition(pos), Quaternion.identity);
        }
    }

    string normalisedTextData(string data)
    {
        var builder = new StringBuilder(data);
        builder.Replace("\"", "");
        builder.Replace(@"\u00A0", " ");

        return builder.ToString().Trim();
    }

    // Update is called once per frame
    void Update () { }
}
