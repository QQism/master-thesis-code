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

    //public float _perspectiveWidthScaleFactor = 1/42.5f;
    public float _perspectiveWidthScaleFactor = 4/174.6323f;
    // public float _perspectiveHeightScaleFactor = 32/42.5f;
    public float _perspectiveHeightScaleFactor = 32/174.6323f;

    public float _landscapeWidth = 0;

    public bool _perspectiveScaling = false;

    public float _distanceToScreenY = 0;

    [SerializeField]
    public GameObject _dataBar;
    [SerializeField]
    public GameObject _frameBar;

    [SerializeField]
    public GameObject _indicationArrow;

    public float Value { get { return _value; } set { _value = value; } }

    public Vector2d LatLong { get { return _latLong; } set { _latLong = value; } }

    public float MaxValue { get { return _maxValue; } set { _maxValue = value; } }

    public float Elevation { get { return _elevation; } set { _elevation = value; } }

    [SerializeField]
    public MeshSelection _meshType;

    public MeshSelection MeshType { get { return _meshType; } set { _meshType = value; } }

    public bool _static = false;

    public Mesh _cubeMesh;
    public Mesh _cylinderMesh;
    public Mesh _quadMesh;

    [Range(0, 2)]
    public int _selectedIdx;

    private Vector3 _originalScale;

    private Material dataMaterial;
    private Material frameMaterial;

    private MapDataPoint _mapDataPoint;
    public MapDataPoint mapDataPoint
    {
        get
        {
            return _mapDataPoint;
        }
        set
        {
            _mapDataPoint = value;
            var arrow = _indicationArrow.GetComponent<ArrowIndicationBehavior>();
            arrow.mapDataPoint = value;
            _mapDataPoint.OnPoseEnter += onPoseEnter;
            _mapDataPoint.OnPoseLeave += onPoseLeave;

            PoseBehavior dataPose = _dataBar.gameObject.AddComponent(typeof(PoseBehavior)) as PoseBehavior;
            if (dataPose)
            {
                dataPose.onPoseEnter += _mapDataPoint.poseEnter;
                dataPose.onPoseLeave += _mapDataPoint.poseLeave;
            }
            PoseBehavior framePose = _frameBar.gameObject.AddComponent(typeof(PoseBehavior)) as PoseBehavior;
            if (framePose)
            {
                framePose.onPoseEnter += _mapDataPoint.poseEnter;
                framePose.onPoseLeave += _mapDataPoint.poseLeave;
            }
        }
    }
    void Start()
    {
        _originalScale = transform.localScale;

        if (_dataBar && _frameBar)
            initializeDataAndFrameComponents();
    }

    void initializeDataAndFrameComponents()
    {
        // Create copies of materials 
        Renderer dataRenderer = _dataBar.GetComponent<Renderer>();
        Renderer frameRenderer = _frameBar.GetComponent<Renderer>();

        dataMaterial = new Material(dataRenderer.sharedMaterial);
        frameMaterial = new Material(frameRenderer.sharedMaterial);

        dataRenderer.sharedMaterial = dataMaterial;
        frameRenderer.sharedMaterial = frameMaterial;
    }

    public void updateBars()
    {
        if (_dataBar == null || _frameBar == null)
            return;

        MeshFilter _dataBarFilter = _dataBar.GetComponent<MeshFilter>();
        MeshFilter _frameBarFilter = _frameBar.GetComponent<MeshFilter>();

        if (_dataBarFilter == null ||
            _dataBarFilter.sharedMesh == null ||
            _frameBarFilter == null ||
            _frameBarFilter.sharedMesh == null)
            return;

        _dataBar.GetComponent<MeshFilter>().mesh = getMesh();
        _frameBar.GetComponent<MeshFilter>().mesh = getMesh();

        var meshHeightScaleFactor = heightScaleFactor();
        scaleMesh(meshHeightScaleFactor);
        var dataCollider = _dataBar.GetComponent<BoxCollider>();
        dataCollider.size = new Vector3(dataCollider.size.x, meshHeightScaleFactor, dataCollider.size.z);

        var frameCollider = _frameBar.GetComponent<BoxCollider>();
        frameCollider.size = new Vector3(frameCollider.size.x, meshHeightScaleFactor, frameCollider.size.z);
        moveBarOffTheGround();

        var arrow = _indicationArrow.GetComponent<ArrowIndicationBehavior>();
    }

    public void shear()
    {
        // Debug.Log("Sheer " + name);
        Mesh mesh = _dataBar.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i=0; i < vertices.Length; i++)
        {
            if (mesh.vertices[i].x> 0) {
                vertices[i].x +=  2 * vertices[i].y; 
            } else {
                vertices[i].x -=  2 * vertices[i].y; 
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }


    void OnValidate()
    {
        //Debug.Log("[" + name + "] Distance to X: " + PlayerCamera.WorldToScreenPoint(Vector3.Scale(transform.position, new Vector3(1, 0, 1))).y.ToString());
        updateBars();
    }

    void Update()
    {
        if (_static == true)
            return;

        if (_perspectiveScaling)
        {
            //updatePerspectiveScale();
            //updateBars();
            // shear();
        }
    }

    void updatePerspectiveScale()
    {
        float distance = (Camera.main.transform.position - transform.position).magnitude;
        float size = distance * 0.0001f * Camera.main.fieldOfView;
        transform.localScale = _originalScale * size;
    }

    void moveBarOffTheGround()
    { 
        //transform.position = new Vector3(transform.position.x, _elevation + _maxHeight/2, transform.position.z);
        transform.position = new Vector3(transform.position.x, _elevation + transform.localScale.y/2, transform.position.z);
        //transform.position = new Vector3(transform.position.x, transform.localScale.y/2, transform.position.z);
    }

    void scaleDataBarToValue(float meshScaleFactor=1)
    {
        //float maxHeight = _maxValue * _barHeightBuffer;
        float maxHeight = _maxValue; // * _barHeightBuffer;
        float scaledAmount = Value / maxHeight / meshScaleFactor;
        _dataBar.transform.localScale = new Vector3(_dataBar.transform.localScale.x, scaledAmount, _dataBar.transform.localScale.z);
        float newY = -0.5f + scaledAmount/2 * meshScaleFactor;
        _dataBar.transform.localPosition = new Vector3(_dataBar.transform.localPosition.x, newY, _dataBar.transform.localPosition.z);
    }

    void scaleFrameBarToValue(float meshScaleFactor=1)
    { 
        //float maxHeight = _maxValue * _barHeightBuffer;
        float maxHeight = _maxValue;// * _barHeightBuffer;
        float scaledAmount = (1 - (Value / maxHeight)) / meshScaleFactor;
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
        float distance = Camera.main.WorldToScreenPoint(Vector3.Scale(transform.position, new Vector3(1, 0, 1))).y;
        //float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        _distanceToScreenY = distance;
        float radFov = Camera.main.fieldOfView * Mathf.Deg2Rad;
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

    void onPoseEnter()
    {
        Debug.Log("Pose enter: " + name);
        if (dataMaterial != null)
            dataMaterial.SetInt("_OutlineOn", 1);

        if (frameMaterial != null)
            frameMaterial.SetInt("_OutlineOn", 1);
        //mapDataPoint.Selected = true;
    }

    void onPoseLeave()
    {
        Debug.Log("Pose leave: " + name);
        if (dataMaterial != null)
            dataMaterial.SetInt("_OutlineOn", 0);

        if (frameMaterial != null)
            frameMaterial.SetInt("_OutlineOn", 0);
        //mapDataPoint.Selected = false;
    }

    Mesh getMesh()
    {
        Mesh mesh;
        switch(_meshType)
        {
            case MeshSelection.Cube:
                mesh = _cubeMesh;
                break;
            case MeshSelection.Quad:
                mesh = _quadMesh;
                break;
            case MeshSelection.Cylinder:
            default:
                mesh = _cylinderMesh;
                break;
        }
        return mesh;
    }
    void OnDestroy()
    {
        if (_mapDataPoint != null)
        {
            _mapDataPoint.OnPoseEnter -= onPoseEnter;
            _mapDataPoint.OnPoseLeave -= onPoseLeave;
        }
    }
}
