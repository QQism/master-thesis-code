using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserResponse 
{
	public int id;
	public int userId {get; set;}
	public Question question;
	public float startTime {get; set;}
	public float completionTime {get; set;}
	public int answer {get; set;}

	public UserResponse(Question question)
	{
		this.question = question;
	}

	public void save()
	{
		Debug.Log("Save Question " + question.id + " - User " + userId + " - StartTime: " + startTime + " - EndTime: " + completionTime);
	}
}
