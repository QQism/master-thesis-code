using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using TMPro;
using System;

using UnityEngine;

public class NumericAnswerPanelBehavior : MonoBehaviour {

	public int answerValue = 50;
	private int defaultAnswerValue = 50;

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

	AudioSource _audioSource;

	AudioClip _tapSound;
	AudioClip _denySound;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		StartCoroutine(loadSounds());
		_activeTextColor = new Color32(153, 50, 204, 255);
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
		if (controller.confirmAnswer())
		{
			StudyPlot.Instance.answer(answerValue);
			resetState();
		}

		if (controller.isIncreasingValueFaster() || controller.isIncreasingValue()) 
		{

			if (answerValue < 100)
			{
				answerValue += 1;
				_audioSource.PlayOneShot(_tapSound);
			} 
			else
			{ 
				controller.triggerHapticPulse(2);
				_audioSource.PlayOneShot(_denySound);
			}
			_changedLastTime = 1;
		}

		if (controller.isDecreasingValueFaster() || controller.isDecreasingValue())
		{
			if (answerValue > 0)
			{
				answerValue -= 1;
				_audioSource.PlayOneShot(_tapSound);
			} 
			else
			{ 
				controller.triggerHapticPulse(2);
				_audioSource.PlayOneShot(_denySound);
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
		} else 
		{
			_upTextMesh.color = _defaultTextColor;
			_downTextMesh.color = _defaultTextColor;
		}
	}

	public void resetState()
	{
		answerValue = defaultAnswerValue;
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
