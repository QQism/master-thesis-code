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

    public void updateBars()
    {
        _dataBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];
        _frameBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];

        moveBarOffTheGround();

        if (_meshType == MeshSelection.Cylinder)
            scaleMesh(2);
        else if (_meshType == MeshSelection.Cube)
            scaleMesh(1);
        else if (_meshType == MeshSelection.Quad)
            scaleMesh(1);
    }

    void OnValidate()
    {
        updateBars();
    }

    void moveBarOffTheGround()
    { 
        transform.position = new Vector3(transform.position.x, _elevation + _maxHeight/2, transform.position.z);
    }

    void scaleDataBarToValue(float scaleFactor=1)
    {
        float scaledAmount = Value / (_maxValue * _barHeightBuffer) / scaleFactor;
        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2 * scaleFactor;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }

    void scaleFrameBarToValue(float scaleFactor=1)
    { 
        float scaledAmount = (1 - (Value / (_maxValue * _barHeightBuffer))) / scaleFactor;
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        // newY = -0.5f + scaledAmount/2 + (1- scaledAmount) = 0.5f - scaledAmount/2
        Vector3 oldPosition = _frameBar.transform.localPosition;
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2 * scaleFactor, oldPosition.z);

    }

    void scaleMesh(float scaleFactor)
    {
        scaleDataBarToValue(scaleFactor);
        scaleFrameBarToValue(scaleFactor);
    }
}
