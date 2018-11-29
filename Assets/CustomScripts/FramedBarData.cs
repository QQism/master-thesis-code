using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramedBarData : MonoBehaviour {

    public float _value;
    public Vector2d _latLong = Vector2d.zero;
    public float _maxHeight;
    public float _barHeightBuffer;
    public float _elevation;
    public float _maxValue;
    public Mesh _barMesh;

    public string _meshType;

    [SerializeField]
    public GameObject _dataBar;
    [SerializeField]
    public GameObject _frameBar;

    public float Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public Vector2d LatLong
    {
        get { return _latLong; }
        set { _latLong = value; }
    }

    public float MaxValue
    {
        get { return _maxValue; }
        set { _maxValue = value; }
    }

    public float Elevation
    {
        get { return _elevation; }
        set { _elevation = value; }
    }

    public Mesh BarMesh
    {
        get { return _barMesh; }
        set
        {
            _barMesh = value;
            _dataBar.GetComponent<MeshFilter>().mesh = _barMesh;
            _frameBar.GetComponent<MeshFilter>().mesh = _barMesh;
        }
    }

    public void updateBars()
    {
        moveBarOffTheGround();

        if (_meshType == MeshSelection.Cylinder.ToString())
            scaleCylinderMesh();
        else if (_meshType == MeshSelection.Cube.ToString())
            scaleCubeMesh();
        else if (_meshType == MeshSelection.Quad.ToString())
            scaleQuadMesh();
    }

    void moveBarOffTheGround()
    { 
        transform.position += new Vector3(0, _elevation + _maxHeight/2, 0);
    }

    void scaleCubeMesh()
    {
        scaleCubeDataBarToValue();
        scaleCubeFrameBarToValue();
    }

    void scaleCubeDataBarToValue()
    {
        float scaledAmount = _value / (_maxValue * _barHeightBuffer);

        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }
    void scaleCubeFrameBarToValue()
    {
        float scaledAmount = 1 - (_value / (_maxValue * _barHeightBuffer));
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        Vector3 oldPosition = _frameBar.transform.localPosition;

        // newY = -0.5f + scaledAmount/2 + (1- scaledAmount) = 0.5f - scaledAmount/2
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2, oldPosition.z);
    }

    void scaleQuadMesh()
    {
        scaleQuadDataBarToValue();
        scaleQuadFrameBarToValue();
    }

    void scaleQuadDataBarToValue()
    {
        float scaledAmount = _value / (_maxValue * _barHeightBuffer);

        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }
    void scaleQuadFrameBarToValue()
    {
        float scaledAmount = 1 - (_value / (_maxValue * _barHeightBuffer));
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        Vector3 oldPosition = _frameBar.transform.localPosition;

        // newY = -0.5f + scaledAmount/2 + (1- scaledAmount) = 0.5f - scaledAmount/2
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2, oldPosition.z);
    }

    void scaleCylinderDataBarToValue()
    {
        float scaledAmount = _value / (_maxValue * _barHeightBuffer);
        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
    }

    void scaleCylinderFrameBarToValue()
    { 
        float scaledAmount = 1 - (_value / (_maxValue * _barHeightBuffer));
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        Vector3 oldPosition = _frameBar.transform.localPosition;
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, _dataBar.transform.localScale.y + _frameBar.transform.localScale.y, oldPosition.z);

    }

    void scaleCylinderMesh()
    {
        scaleCylinderDataBarToValue();
        scaleCylinderFrameBarToValue();
    }
}
