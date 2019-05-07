using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public enum ControllerMode {
	AdjustCone,
	AnswerBoard,
	ShootingAnswer
}
public class ControllerBehavior : MonoBehaviour {

	public MonoBehaviour _attachedCone;
	public MonoBehaviour _attachedPanel;
	public SteamVR_Input_Sources _controller;
    public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean increaseAngleAction;// = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("IncreaseConeAngle");
	public SteamVR_Action_Boolean decreaseAngleAction;// = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("DecreaseConeAngle");

	public SteamVR_Action_Boolean increaseValue;
	public SteamVR_Action_Boolean decreaseValue;
	public SteamVR_Action_Boolean confirmAnswer;

	public SteamVR_Action_Boolean pressDpadLong;

	public SteamVR_Action_Boolean holdingGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("HoldingGrip");

	public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

	public GameObject debugPoseMarkerStart;
	public GameObject debugPoseMarkerEnd;

	public GameObject lastHitObject = null;

	public ControllerMode _controllerMode = ControllerMode.AnswerBoard;

	private Vector3 vectorFoward;

    // Use this for initialization
    void Start()
    {

		// The pose is more towards to ground (30deg) than the actual pointing
		// for the egonomic reason 
        var alpha = 30;
        var y = -Mathf.Sin(alpha * Mathf.Deg2Rad);
        var z = Mathf.Cos(alpha * Mathf.Deg2Rad);
        vectorFoward = new Vector3(0, y, z); // instead of Vector3.forward
    }
	
	// Update is called once per frame
	void Update () 
	{ 
		if (_attachedCone != null && _controllerMode == ControllerMode.AdjustCone)
		{
			_attachedCone.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
		} else if (_attachedPanel != null && _controllerMode == ControllerMode.AnswerBoard)
		{
			if (pressDpadLong.GetState(_controller))
			{
				Debug.Log("Press Long");
			}
			_attachedPanel.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
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
				Vector3 direction = poseAction.GetLocalRotation(_controller) * vectorFoward * 0.3f;
                line.SetPosition(1, line.transform.position + direction);
				debugPoseMarkerEnd.transform.position = line.transform.position + direction;
				Ray ray = new Ray(line.transform.position, direction);
				if (Physics.Raycast(ray, out hit)) 
				{
					//Debug.Log("Hit!");
					debugPoseMarkerEnd.transform.position = hit.point;
					line.SetPosition(1, hit.point);
					handleHit(hit);
				} else
				{
					//Debug.Log("Missed!");
					debugPoseMarkerEnd.transform.position = line.transform.position + direction;
					handleMiss();
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


	public bool isIncreasingValue()
	{
		return increaseValue.GetStateDown(_controller);
	}
	public bool isDecreasingValue()
	{
		return decreaseValue.GetStateDown(_controller);
	}

	public bool isLongPressingDpad()
	{
		return pressDpadLong.GetState(_controller);
	}

	public bool isIncreasingValueFaster()
	{
		return isLongPressingDpad() && increaseValue.GetState(_controller);
	}

	public bool isDecreasingValueFaster()
	{
		return isLongPressingDpad() && decreaseValue.GetState(_controller);
	}

	void handleHit(RaycastHit hit)
	{
		GameObject newHitObject = hit.transform.gameObject;

		if (newHitObject == lastHitObject)
			return;

        if (lastHitObject != null)
        {
			var poseBehavior = lastHitObject.GetComponent<PoseBehavior>();
			if (poseBehavior)
				poseBehavior.poseLeave();
        }

		if (newHitObject != null)
		{
			var poseBehavior = newHitObject.GetComponent<PoseBehavior>();
			if (poseBehavior)
				poseBehavior.poseEnter();

			lastHitObject = newHitObject;
		}
	}

	void handleMiss()
	{
        if (lastHitObject != null)
        {
			var poseBehavior = lastHitObject.GetComponent<PoseBehavior>();
			if (poseBehavior)
				poseBehavior.poseLeave();

			lastHitObject = null;
        }
	}
}
