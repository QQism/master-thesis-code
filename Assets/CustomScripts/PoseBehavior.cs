using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseBehavior : MonoBehaviour {


	public delegate void PoseEnterAction();
	public delegate void PoseLeaveAction();
	public event PoseEnterAction onPoseEnter;
	public event PoseLeaveAction onPoseLeave;

	public bool onPose = false;

	void Awake()
	{
		onPoseEnter += (() => {
			onPose = true;
		});

		onPoseLeave += (() => {
			onPose = false;
		});
	}

	public void poseEnter()
	{
		onPoseEnter();
	}

	public void poseLeave()
	{
		onPoseLeave();
	}
}
