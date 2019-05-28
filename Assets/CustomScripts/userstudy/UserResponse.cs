using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class UserResponse 
{
	public int id;
	public int userId {get; set;}
	public Question question;
	public float startTime {get; set;}
	public float completionTime {get; set;}
	public int answer {get; set;}

	public UserResponse(int userId, Question question)
	{
		this.userId = userId;
		this.question = question;
	}

	public void save()
	{
        Debug.Log("Save Question " + question.id +
        " - User " + userId + "answered " + answer + 
		" - Question Answer " + question.getAnswer() + 
        " - StartTime: " + startTime +
        " - EndTime: " + completionTime);
		string fileName = getFileName();
		bool fileExists = File.Exists(fileName);
		FileStream fs;
		StreamWriter sw;
        using (fs = new FileStream(fileName, fileExists ? FileMode.Append : FileMode.Create))
        {
            sw = new StreamWriter(fs);
            if (!fileExists)
            {
                // Add headers
                string[] headerData = new string[] {
                    "User",
                    "Dataset",
                    "QuestionId",
                    "Task",
                    "Vis",
                    "Answer",
                    "Resposne",
                    "StartTime",
                    "EndTime",
            	};
            	sw.WriteLine(String.Join(", ", headerData));
            }

            string[] data = new string[] {
                userId.ToString(),
                question.dataset.ToString(),
                question.id.ToString(),
                question.task.ToString(),
                question.visualisationType.ToString(),
                question.getAnswer().ToString(),
                answer.ToString(),
                startTime.ToString(),
                completionTime.ToString()
            };
            sw.WriteLine(String.Join(", ", data));
            sw.Close();
            fs.Close();
        }
	}

    public string getFileName()
    {
        return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] {
            Application.dataPath,
            "Results",
            userId.ToString()+ "_" + "_" + question.task.ToString() + ".csv"
        });
    }
}
