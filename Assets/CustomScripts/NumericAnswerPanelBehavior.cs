using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class NumericAnswerPanelBehavior : MonoBehaviour {

	public int answerValue = 50;

	[SerializeField]
	public GameObject upText;

	[SerializeField]
	public GameObject valueText;

	[SerializeField]
	public GameObject downText;

	private int _changedLastTime = 0;

	private TextMeshProUGUI _valueTextMesh;
	private TextMeshProUGUI _upTextMesh;
	private TextMeshProUGUI _downTextMesh;

	private Color32 _defaultTextColor;
	private Color32 _activeTextColor;
	// Use this for initialization

	void Awake()
	{
		_activeTextColor = new Color32(224,255,255, 255);
		_valueTextMesh = valueText.GetComponent<TextMeshProUGUI>();
		_upTextMesh = upText.GetComponent<TextMeshProUGUI>();
		_downTextMesh = downText.GetComponent<TextMeshProUGUI>();
		_defaultTextColor = _upTextMesh.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		_valueTextMesh.text = answerValue.ToString() + "%";
		//gameObject.transform.LookAt(Camera.main.transform);
	}

	public void controlerUpdate(ControllerBehavior controller) 
	{
		if (controller.isIncreasingValueFaster() || controller.isIncreasingValue()) 
		{

			if (answerValue < 100)
			{
				answerValue += 1;
			} else
			{ 
				controller.triggerHapticPulse(2);
			}
			_changedLastTime = 1;
		}

		if (controller.isDecreasingValueFaster() || controller.isDecreasingValue())
		{
			if (answerValue > 0)
			{
				answerValue -= 1;
			} else
			{ 
				controller.triggerHapticPulse(2);
			}
			_changedLastTime = -1;
		}

		if (_changedLastTime != 0)
		{
			if (_changedLastTime > 0)
				_upTextMesh.color = _activeTextColor;
			else
				_downTextMesh.color = _activeTextColor;
			_changedLastTime = 0;
		} else {
			_upTextMesh.color = _defaultTextColor;
			_downTextMesh.color = _defaultTextColor;
		}
	}
	
}
