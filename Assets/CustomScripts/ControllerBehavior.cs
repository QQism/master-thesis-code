using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ControllerBehavior : MonoBehaviour {

	public MonoBehaviour _attachedCone;
	public SteamVR_Input_Sources _controller;
    public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean increaseAngleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("IncreaseConeAngle");
	public SteamVR_Action_Boolean decreaseAngleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("DecreaseConeAngle");

	public SteamVR_Action_Boolean increaseHeightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("IncreaseConeHeight");
	public SteamVR_Action_Boolean decreaseHeightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("DecreaseConeHeight");

	private LongPressDetector _increaseAngleLongPressDetector;
	// Use this for initialization
	void Start () 
	{
		_increaseAngleLongPressDetector = new LongPressDetector(increaseAngleAction, _controller);
	}
	
	// Update is called once per frame
	void Update () {
		if (_attachedCone != null)
		{
			_attachedCone.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
		}

	}

	public bool isIncreasingAngle()
	{
		return increaseAngleAction.GetStateDown(_controller);
	}

	public bool isDecreasingAngle()
	{
		return decreaseAngleAction.GetStateDown(_controller);
	}

	public bool isIncreasingHeight()
	{
		return increaseHeightAction.GetStateDown(_controller);
	}

	public bool isDecreasingHeight()
	{
		return decreaseHeightAction.GetStateDown(_controller);
	}

	public bool isIncreasingAngleLongPress()
	{
		return _increaseAngleLongPressDetector.isLongPressing(Time.time);
	}
}
