using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LongPressDetector {

	private SteamVR_Action_Boolean _action;
	private SteamVR_Input_Sources _controller;
	private bool _isLongPressing = false;
	private float longPressStartTime = 0;

	private float thresholdDuration = 10.0f;

	public LongPressDetector(SteamVR_Action_Boolean action, SteamVR_Input_Sources controller)
	{
		_action = action;
		_controller = controller;
	}

	public bool isLongPressing(float time) 
	{	
		if (_action.GetStateUp(_controller))
		{
			longPressStartTime = Time.time;
		}

		if (_action.GetLastState(_controller))
		{
			if (Time.time - longPressStartTime > thresholdDuration)
			{
				_isLongPressing = true;
			} else
			{ 
				_isLongPressing = false;
			}
        }
        else
        {
            _isLongPressing = false;
            longPressStartTime = 0;
        }

		return _isLongPressing;
	}
}
