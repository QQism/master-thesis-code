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

	public SteamVR_Action_Boolean holdingGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("HoldingGrip");

	public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

	public GameObject debugPoseMarkerStart;
	public GameObject debugPoseMarkerEnd;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
		if (_attachedCone != null)
		{
			_attachedCone.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
		}

		if (debugPoseMarkerStart != null && poseAction.GetActive(_controller))
		{
			Vector3 posePos = poseAction.GetLocalPosition(_controller);
			debugPoseMarkerStart.transform.localPosition = posePos;
			LineRenderer line = debugPoseMarkerStart.GetComponent<LineRenderer>();
			line.transform.position = debugPoseMarkerStart.transform.position;
			
			if (line != null)
			{
				line.SetPosition(0, debugPoseMarkerStart.transform.position);
				line.transform.localRotation = poseAction.GetLocalRotation(_controller);

				RaycastHit hit;
				// Shorten the default magnitude of the beam by 0.3
				Vector3 direction = poseAction.GetLocalRotation(_controller) * Vector3.forward * 0.3f;
                line.SetPosition(1, line.transform.position + direction);
				debugPoseMarkerEnd.transform.position = line.transform.position + direction;
				Ray ray = new Ray(line.transform.position, direction);
				if (Physics.Raycast(ray, out hit)) 
				{
					//Debug.Log("Hit!");
					debugPoseMarkerEnd.transform.position = hit.point;
					line.SetPosition(1, hit.point);
				} else
				{
					//Debug.Log("Missed!");
					debugPoseMarkerEnd.transform.position = line.transform.position + direction;
				}
				
			}
		}

	}

	public bool isIncreasingAngle()
	{
		return increaseAngleAction.GetState(_controller) && !holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingAngle()
	{
		return decreaseAngleAction.GetState(_controller) && !holdingGripAction.GetLastState(_controller);
	}

	
	public bool isIncreasingHeight()
	{
		return increaseAngleAction.GetState(_controller) && holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingHeight()
	{
		return decreaseAngleAction.GetState(_controller) && holdingGripAction.GetLastState(_controller); 
	}

	public bool isIncreasingInnerCirle()
	{
		return increaseAngleAction.GetState(_controller) && holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingInnerCircle()
	{
		return decreaseAngleAction.GetState(_controller) && holdingGripAction.GetLastState(_controller); 
	}

	public void triggerHapticPulse(float duration)
	{
		hapticAction.Execute(0, duration, 1f/duration, 1, _controller);
	}
}
