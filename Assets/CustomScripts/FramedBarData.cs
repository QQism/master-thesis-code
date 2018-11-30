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

    [SerializeField]
    public GameObject _dataBar;
    [SerializeField]
    public GameObject _frameBar;

    public Dictionary<MeshSelection, Mesh> AvailableMeshes { get; set; }

    public float Value { get { return _value; } set { _value = value; } }

    public Vector2d LatLong { get { return _latLong; } set { _latLong = value; } }

    public float MaxValue { get { return _maxValue; } set { _maxValue = value; } }

    public float Elevation { get { return _elevation; } set { _elevation = value; } }

    [SerializeField]
    public MeshSelection _meshType;

    public MeshSelection MeshType { get { return _meshType; } set { _meshType = value; } }

    private float _meshScaleFactor = 1;

    public void updateBars()
    {
        _dataBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];
        _frameBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];

        moveBarOffTheGround();

        if (_meshType == MeshSelection.Cylinder)
        { 
            _meshScaleFactor = 2;
            scaleCylinderMesh();
        }
        else if (_meshType == MeshSelection.Cube)
        { 
            _meshScaleFactor = 1;
            scaleCylinderMesh();
        }
        else if (_meshType == MeshSelection.Quad)
        { 
            _meshScaleFactor = 1;
            scaleQuadMesh();
        }
            
        //scaleCylinderMesh();
    }

    void OnValidate()
    {
        updateBars();
    }

    void moveBarOffTheGround()
    { 
        transform.position = new Vector3(transform.position.x, _elevation + _maxHeight/2, transform.position.z);
    }

    void scaleCubeMesh()
    {
        scaleCubeDataBarToValue();
        scaleCubeFrameBarToValue();
    }

    void scaleCubeDataBarToValue()
    {
        float scaledAmount = Value / (_maxValue * _barHeightBuffer);

        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }
    void scaleCubeFrameBarToValue()
    {
        float scaledAmount = 1 - (Value / (_maxValue * _barHeightBuffer));
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
        float scaledAmount = Value / (_maxValue * _barHeightBuffer);

        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }
    void scaleQuadFrameBarToValue()
    {
        float scaledAmount = 1 - (Value / (_maxValue * _barHeightBuffer));
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        Vector3 oldPosition = _frameBar.transform.localPosition;

        // newY = -0.5f + scaledAmount/2 + (1- scaledAmount) = 0.5f - scaledAmount/2
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2, oldPosition.z);
    }

    void scaleCylinderDataBarToValue()
    {
        float scaledAmount = Value / (_maxValue * _barHeightBuffer) / _meshScaleFactor;
        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2 * _meshScaleFactor;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }

    void scaleCylinderFrameBarToValue()
    { 
        float scaledAmount = (1 - (Value / (_maxValue * _barHeightBuffer))) / _meshScaleFactor;
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        Vector3 oldPosition = _frameBar.transform.localPosition;
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2 * _meshScaleFactor, oldPosition.z);

    }

    void scaleCylinderMesh()
    {
        scaleCylinderDataBarToValue();
        scaleCylinderFrameBarToValue();
    }
}
