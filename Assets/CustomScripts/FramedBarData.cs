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

    public float _perspectiveWidthScaleFactor = 1/42.5f;
    public float _perspectiveHeightScaleFactor = 32/42.5f;

    public float _landscapeWidth = 0;

    public bool _perspectiveScaling = false;

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

    public Camera PlayerCamera { get; set; }

    public void updateBars()
    {
        _dataBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];
        _frameBar.GetComponent<MeshFilter>().mesh = AvailableMeshes[_meshType];

        moveBarOffTheGround();
        scaleMesh(heightScaleFactor());
    }

    void OnValidate()
    {
        updateBars();
    }

    void Update()
    {
        if (_perspectiveScaling)
        {
            updatePerspectiveScale();
            updateBars();
        }
    }

    void updatePerspectiveScale()
    {
        _landscapeWidth = calculateLandscapeWidth();
        float newScaleX, newScaleY, newScaleZ;
        newScaleX = newScaleZ = _landscapeWidth * _perspectiveWidthScaleFactor;
        newScaleY = _landscapeWidth * _perspectiveHeightScaleFactor / heightScaleFactor();
        transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
    }

    void moveBarOffTheGround()
    { 
        transform.position = new Vector3(transform.position.x, _elevation + _maxHeight/2, transform.position.z);
    }

    void scaleDataBarToValue(float meshScaleFactor=1)
    {
        float scaledAmount = Value / (_maxValue * _barHeightBuffer) / meshScaleFactor;
        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2 * meshScaleFactor;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }

    void scaleFrameBarToValue(float meshScaleFactor=1)
    { 
        float scaledAmount = (1 - (Value / (_maxValue * _barHeightBuffer))) / meshScaleFactor;
        Vector3 oldScale = _frameBar.transform.localScale;
        _frameBar.transform.localScale = new Vector3(oldScale.x, scaledAmount, oldScale.z);

        // newY = -0.5f + scaledAmount/2 + (1- scaledAmount) = 0.5f - scaledAmount/2
        Vector3 oldPosition = _frameBar.transform.localPosition;
        _frameBar.transform.localPosition = new Vector3(oldPosition.x, 0.5f - scaledAmount/2 * meshScaleFactor, oldPosition.z);

    }

    void scaleMesh(float meshScaleFactor)
    {
        scaleDataBarToValue(meshScaleFactor);
        scaleFrameBarToValue(meshScaleFactor);
    }

    public float calculateLandscapeWidth()
    {
        float distance = Vector3.Distance(PlayerCamera.transform.position, transform.position);
        float radFov = PlayerCamera.fieldOfView * Mathf.Deg2Rad;
        return distance * 2 * Mathf.Sin(Mathf.PI/2.0f - radFov) / Mathf.Sin(radFov);
    }

    float heightScaleFactor()
    {
        switch (_meshType)
        {
            case MeshSelection.Cylinder:
                return 2.0f;
            default:
                return 1.0f;
        }
    }
}
