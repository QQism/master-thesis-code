using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataPointsGenerator: MonoBehaviour {
	private float numberDataPoints = 100;
	private float furthestLeft = -1;
	private float furthestRight = 1;

	private float furthestForward = 1;
	private float furthestBack = -1;

	private float lowestValue = 0;

	private float highestValue = 1;

	private bool generated = false;

	void Start() 
	{
	}

     public static string getDatasetFilename() {
         return string.Format("{0}/Datasets/data_{1}.csv", 
                              Application.dataPath, 
                              System.DateTime.Now.ToString("yyyyMMddHHmmss"));
     }
	
	// Update is called once per frame
	void Update ()
	{
		generateData();
	}

    void generateData()
    {
		generated |= Input.GetKeyDown("k");
        if (generated)
        {
			string filename = getDatasetFilename();
            StreamWriter writer = new StreamWriter(filename, true);
            for (int i = 0; i < numberDataPoints; i++)
            {
                float x = Random.Range(furthestLeft, furthestRight);
                float y = Random.Range(furthestBack, furthestForward);
                float val = Random.Range(lowestValue, highestValue);
                writer.WriteLine(i.ToString() + ", " + x.ToString() + ", " + y.ToString() + ", " + val.ToString());
            }
            writer.Close();
            Debug.Log(string.Format("Dateset generated: {0}", filename));
			generated = false;
        }
    }
}
