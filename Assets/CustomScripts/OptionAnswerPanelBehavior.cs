using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionAnswerPanelBehavior : MonoBehaviour {

	[SerializeField]
	public GameObject upTick;

	[SerializeField]
	public GameObject downTick;

	public int optionSelected = BarOption.Bar1;
	private int defaultOption = BarOption.Bar1;

	private Transform upTickTransform;
	private Transform downTickTransform;

	private float leftX;
	private float rightX;

	void Awake () 
	{
		upTickTransform = upTick.GetComponent<RectTransform>().transform;	
		downTickTransform = downTick.GetComponent<RectTransform>().transform;
		leftX = -0.23f;
		rightX = -leftX;
	}
	
	// Update is called once per frame
	void Update () 
	{ 
	}

	public void controlerUpdate(ControllerBehavior controller) 
	{
		if (controller.confirmAnswer())
		{
			StudyPlot.Instance.answer(optionSelected);
		}

		if (controller.isSelectingLeft())
		{
            if (optionSelected != BarOption.Bar1)
            {
				selectOption(BarOption.Bar1);
            }
            else
            {
                controller.triggerHapticPulse(2);
            }
        }

        if (controller.isSelectingRight())
		{
			if (optionSelected != BarOption.Bar2)
			{ 
				selectOption(BarOption.Bar2);
			}
			else
			{	
				controller.triggerHapticPulse(2);
			}

		}
	}

	private void selectOption(int value)
    {
        optionSelected = value;
		if (optionSelected == BarOption.Bar1)
		{
			moveTransform(upTickTransform, true);
			moveTransform(downTickTransform, true);
		} else if (optionSelected == BarOption.Bar2)
		{ 
			moveTransform(upTickTransform, false);
			moveTransform(downTickTransform, false);
		}
    }

	public void resetState()
	{
		selectOption(defaultOption);
	}

    private void moveTransform(Transform transform, bool isLeft)
    {
        if (isLeft)
            transform.localPosition = new Vector3(leftX, transform.localPosition.y, transform.localPosition.z);
		else
            transform.localPosition = new Vector3(rightX, transform.localPosition.y, transform.localPosition.z);
    }
}
