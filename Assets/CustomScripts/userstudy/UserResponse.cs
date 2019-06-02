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
        switch(question.task)
        {
            case Task.EstimateSinglePoint:
                printTask1Log();
                break;
            case Task.PickLargerDataPoint:
                printTask2Log();
                break;
            case Task.PickCloserDataPoint:
                printTask3Log();
                break;
        }

		string fileName = getFileName();
		bool fileExists = File.Exists(fileName);
		FileStream fs;
		StreamWriter sw;
        using (fs = new FileStream(fileName, fileExists ? FileMode.Append : FileMode.Create))
        {
            sw = new StreamWriter(fs);
            if (!fileExists)
            {
                string[] headerData = null;
                switch (question.task)
                {
                    case Task.EstimateSinglePoint:
                        headerData = getTask1Header();
                        break;
                    case Task.PickLargerDataPoint:
                        headerData = getTask2Header();
                        break;
                    case Task.PickCloserDataPoint:
                        headerData = getTask3Header();
                        break;
                }
                // Add headers
            	sw.WriteLine(String.Join(", ", headerData));
            }

            string[] data = null;
            switch (question.task)
            {
                case Task.EstimateSinglePoint:
                    data = getTask1LogLine();
                    break;
                case Task.PickLargerDataPoint:
                    data = getTask2LogLine();
                    break;
                case Task.PickCloserDataPoint:
                    data = getTask3LogLine();
                    break;
            }

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
            userId.ToString()+ "_" + "_" + question.task.ToString() + "_" + question.dataset.ToString() + ".csv"
        });
    }

    void printTask1Log()
    {
        Debug.Log("Save Question: " + question.id +
        ", Code: " + question.code + 
        ", User: " + userId  +
		", Answer: " + question.getAnswer() + 
        ", Response: " + answer +
        ", StartTime: " + startTime +
        ", EndTime: " + completionTime +
        ", PointName: " + question.getDataPoint1().Name);
    }

    void printTask2Log()
    {
        Debug.Log("Save Question " + question.id +
        ", Code: " + question.code +
        ", User: " + userId +
        ", Answer: " + question.getAnswer() +
        ", Response: " + answer +
        ", StartTime: " + startTime +
        ", EndTime: " + completionTime + 
        ", Point1Name: " + question.getDataPoint1().Name +
        ", Point2Name: " + question.getDataPoint2().Name +
        ", Point1Value: " + question.getDataPoint1().Value +
        ", Point2value: " + question.getDataPoint2().Value +
        ", Point1Distance: " + question.getDataPoint1().RawPosition +
        ", Point2Distance: " + question.getDataPoint2().RawPosition);
    }

    void printTask3Log()
    {
        printTask2Log();
    }

    string[] getTask1Header()
    {
        return new string[] {
                    "User",
                    "Dataset",
                    "QuestionId",
                    "Code",
                    "Task",
                    "Vis",
                    "Answer",
                    "Resposne",
                    "StartTime",
                    "EndTime",
                    "PointName"};
    }

    string[] getTask1LogLine()
    {
        return new string[] {
                userId.ToString(),
                question.dataset.ToString(),
                question.id.ToString(),
                question.code,
                question.task.ToString(),
                question.visualisationType.ToString(),
                question.getAnswer().ToString(),
                answer.ToString(),
                startTime.ToString(),
                completionTime.ToString(),
                question.getDataPoint1().Name,
            };
    }

    string[] getTask2Header()
    {
        return new string[] {
                    "User",
                    "Dataset",
                    "QuestionId",
                    "Code",
                    "Task",
                    "Vis",
                    "Answer",
                    "Resposne",
                    "StartTime",
                    "EndTime",
                    "Point1Name", 
                    "Point2Name", 
                    "Point1Value", 
                    "Point2value", 
                    "Point1Distance", 
                    "Point2Distance",
                    };
    }

    string[] getTask2LogLine()
    {
        var point1 = question.getDataPoint1();
        var point2 = question.getDataPoint2();

        return new string[] {
                userId.ToString(),
                question.dataset.ToString(),
                question.id.ToString(),
                question.code,
                question.task.ToString(),
                question.visualisationType.ToString(),
                question.getAnswer().ToString(),
                answer.ToString(),
                startTime.ToString(),
                completionTime.ToString(),
                point1.Name,
                point2.Name,
                point1.Value.ToString(),
                point2.Value.ToString(),
                point1.getDistance().ToString(),
                point2.getDistance().ToString(),
            };
    }

    string[] getTask3Header()
    {
        return getTask2Header();
    }

    string[] getTask3LogLine()
    {
        return getTask2LogLine();
    }
}
