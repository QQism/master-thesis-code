using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public enum ControllerMode {
	AdjustCone,
	NumericAnswerBoard,
	OptionAnswerBoard,
	ShootingAnswer
}
public class ControllerBehavior : MonoBehaviour {

	public MonoBehaviour _attachedCone;
	public MonoBehaviour _attachedNumericPanel;
	public MonoBehaviour _attachedOptionPanel;
	public SteamVR_Input_Sources _controller;
    public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");
	public SteamVR_Action_Boolean padUpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadUp");
	public SteamVR_Action_Boolean padDownAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadDown");
	public SteamVR_Action_Boolean padLeftAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadLeft");
	public SteamVR_Action_Boolean padRightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadRight");
	public SteamVR_Action_Boolean pressTrigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PressTrigger");
	public SteamVR_Action_Boolean holdingGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("HoldingGrip");
	public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

	public GameObject debugPoseMarkerStart;
	public GameObject debugPoseMarkerEnd;

	public GameObject lastHitObject = null;

	public ControllerMode _controllerMode = ControllerMode.NumericAnswerBoard;

	private Vector3 vectorFoward;

	private Dictionary<SteamVR_Action_Boolean, float> longPresses = new Dictionary<SteamVR_Action_Boolean, float>();

    // Use this for initialization
    void Awake()
    {
		// The pose is more towards to ground (30deg) than the actual pointing
		// for the egonomic reason 
        var alpha = 30;
        var y = -Mathf.Sin(alpha * Mathf.Deg2Rad);
        var z = Mathf.Cos(alpha * Mathf.Deg2Rad);
        vectorFoward = new Vector3(0, y, z); // instead of Vector3.forward

		//longPresses.Add(increaseAngleAction, 0);
		//longPresses.Add(decreaseAngleAction, 0);
		longPresses.Add(padUpAction, 0);
		longPresses.Add(padDownAction, 0);
		longPresses.Add(padLeftAction, 0);
		longPresses.Add(padRightAction, 0);
    }
	
	// Update is called once per frame
	void Update () 
	{ 
		if (_attachedCone != null && _controllerMode == ControllerMode.AdjustCone)
		{
			_attachedCone.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
		}

		if (_attachedNumericPanel != null)
		{
			if (_controllerMode == ControllerMode.NumericAnswerBoard)
			{
				_attachedNumericPanel.gameObject.SetActive(true);
				_attachedNumericPanel.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				_attachedNumericPanel.SendMessage("resetState", SendMessageOptions.DontRequireReceiver);
				_attachedNumericPanel.gameObject.SetActive(false);
			}
		}

		if (_attachedOptionPanel != null)
		{
            if (_controllerMode == ControllerMode.OptionAnswerBoard)
            {
				_attachedOptionPanel.gameObject.SetActive(true);
                _attachedOptionPanel.SendMessage("controlerUpdate", this, SendMessageOptions.DontRequireReceiver);
            }
			else
			{
				_attachedOptionPanel.SendMessage("resetState", SendMessageOptions.DontRequireReceiver);
				_attachedOptionPanel.gameObject.SetActive(false);
			}
		}

		handlePose();
		checkLongPresses(Time.time);
	}

	void checkLongPresses(float time)
	{
		var actions = new List<SteamVR_Action_Boolean>(longPresses.Keys);
		foreach(var action in actions) 
		{
			if (action.GetActive(_controller)) 
			{
				if (action.GetStateDown(_controller))
					longPresses[action] = time;
				
				if (action.GetStateUp(_controller))
					longPresses[action] = 0;
			}
		}
	}

	private void handlePose()
	{
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
		return padUpAction.GetState(_controller) && !holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingAngle()
	{
		return padDownAction.GetState(_controller) && !holdingGripAction.GetLastState(_controller);
	}

	
	public bool isIncreasingHeight()
	{
		return padUpAction.GetState(_controller) && holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingHeight()
	{
		return padDownAction.GetState(_controller) && holdingGripAction.GetLastState(_controller); 
	}

	public bool isIncreasingInnerCirle()
	{
		return padUpAction.GetState(_controller) && holdingGripAction.GetLastState(_controller);
	}

	public bool isDecreasingInnerCircle()
	{
		return padDownAction.GetState(_controller) && holdingGripAction.GetLastState(_controller); 
	}

	public void triggerHapticPulse(float duration)
	{
		hapticAction.Execute(0, duration, 1f/duration, 1, _controller);
	}


	public bool isIncreasingValue()
	{
		return padUpAction.GetStateDown(_controller);
	}
	public bool isDecreasingValue()
	{
		return padDownAction.GetStateDown(_controller);
	}

	public bool isLongPressing(SteamVR_Action_Boolean action)
	{
		if (!longPresses.ContainsKey(action))
			return false;

		float sinceUp = longPresses[action];
		if (sinceUp > 0 && action.GetState(_controller))
        {
            float deltaTime = 0.25f;
            float duration = Time.time - sinceUp;
			return duration > deltaTime;
        }
		return false;
	}

	public bool isIncreasingValueFaster()
	{
		return isLongPressing(padUpAction);
	}

	public bool isDecreasingValueFaster()
	{
		return isLongPressing(padDownAction);
	}

	public bool isSelectingLeft()
	{
		return padLeftAction.GetStateDown(_controller);
	}
	public bool isSelectingRight()
	{
		return padRightAction.GetStateDown(_controller);
	}

	public bool confirmAnswer()
	{
		return pressTrigger.GetStateDown(_controller);
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
