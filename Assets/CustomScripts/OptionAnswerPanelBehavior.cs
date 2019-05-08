using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionAnswerPanelBehavior : MonoBehaviour {

	[SerializeField]
	public GameObject upTick;

	[SerializeField]
	public GameObject downTick;

	public int optionSelected = 1;

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
	void Update () { }

	public void controlerUpdate(ControllerBehavior controller) 
	{
		if (controller.isSelectingLeft())
		{
            if (optionSelected != 1)
            {
                optionSelected = 1;
            }
            else
            {
                controller.triggerHapticPulse(2);
            }
			moveTransform(upTickTransform, true);
			moveTransform(downTickTransform, true);
        }

        if (controller.isSelectingRight())
		{
			if (optionSelected != 2)
			{ 
				optionSelected = 2;
			}
			else
			{	
				controller.triggerHapticPulse(2);
			}

			moveTransform(upTickTransform, false);
			moveTransform(downTickTransform, false);
		}
	}

    private void moveTransform(Transform transform, bool isLeft)
    {
        if (isLeft)
            transform.localPosition = new Vector3(leftX, transform.localPosition.y, transform.localPosition.z);
		else
            transform.localPosition = new Vector3(rightX, transform.localPosition.y, transform.localPosition.z);
    }
}
