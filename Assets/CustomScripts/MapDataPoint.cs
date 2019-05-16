using System.Collections;
using System.Collections.Generic;
using System;
using Mapbox.Utils;
using UnityEngine;

public class MapDataPoint {
	public string Name {get; set;}
	public Vector2d GeoPosition {get; set;}
	public Vector3 WorldPosition {get; set;} 
	public Vector2 ConePosition {get; set;}
	public float Value {get; set;}

    public bool Selected { get; set; }

    public event Action OnPoseEnter = delegate { };
    public event Action OnPoseLeave = delegate { };
    public event Action OnQuestionOption1ToShow = delegate { };
    public event Action OnQuestionOption2ToShow = delegate { };

	public event Action OnQuestionOption1Completed = delegate { };
	public event Action OnQuestionOption2Completed = delegate { };

	public void poseEnter()
	{
		OnPoseEnter();
	}

	public void poseLeave()
	{
		OnPoseLeave();
	}

	public void showQuestionOption1()
	{
		OnQuestionOption1ToShow();
	}

	public void showQuestionOption2()
	{
		OnQuestionOption2ToShow();
	}

	public void completeQuestionOption1()
	{
		OnQuestionOption1Completed();
	}

	public void completeQuestionOption2()
	{
		OnQuestionOption2Completed();
	}

	public enum MapDataPointState 
	{
		Question1,
		Question2,
		NotQuestion
	}
}
