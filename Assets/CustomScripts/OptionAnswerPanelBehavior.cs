using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
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

	AudioSource _audioSource;

	AudioClip _tapSound;
	AudioClip _denySound;

	void Awake () 
	{
		_audioSource = GetComponent<AudioSource>();
		StartCoroutine(loadSounds());
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
				_audioSource.PlayOneShot(_tapSound);
            }
            else
            {
                controller.triggerHapticPulse(2);
				_audioSource.PlayOneShot(_denySound);
            }
        }

        if (controller.isSelectingRight())
		{
			if (optionSelected != BarOption.Bar2)
			{ 
				selectOption(BarOption.Bar2);
				_audioSource.PlayOneShot(_tapSound);
			}
			else
			{	
				controller.triggerHapticPulse(2);
				_audioSource.PlayOneShot(_denySound);
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

    IEnumerator loadSounds()
    {
        string tapPanelSoundPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            "Sounds",
            "tap_panel.wav"
        });
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(tapPanelSoundPath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                _tapSound = DownloadHandlerAudioClip.GetContent(www);
            }
        }

        string denySoundPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            "Sounds",
            "tap_panel_deny.wav"
        });

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(denySoundPath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                _denySound = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
