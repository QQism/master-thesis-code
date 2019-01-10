using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungarianAlgorithm
{
    public static Dictionary<GameObject, MapDataPoint> Allocate(List<GameObject> bars, List<MapDataPoint> dataPoints)
    {
        int i, j;

        // Initialise the matrix
        float[][] matrix = new float[bars.Count][];

        for (i = 0; i < bars.Count; i++)
            matrix[i] = new float[dataPoints.Count];

        for (i = 0; i < bars.Count; i++)
        {
            var minDistance = float.MaxValue;
            var bar = bars[i];
            for (j = 0; j < dataPoints.Count; j++)
            {
                var point = dataPoints[j];
                var distance = CalculateCost(bar, point);
                matrix[i][j] = distance;

                if (distance < minDistance)
                    minDistance = distance;
            }

            for (j = 0; j < dataPoints.Count; j++)
                matrix[i][j] -= minDistance;
        }

        return new Dictionary<GameObject, MapDataPoint>();
    }

    static float CalculateCost(GameObject obj, MapDataPoint point)
    {
        return Vector3.Distance(obj.transform.position, point.WorldPosition);
    }

    static bool Assignable(float[][] matrix)
    {
        int i, j;
        bool assignable = true;

        for (i = 0; i < matrix.Length; i++)
            for (j = 0; j < matrix[i].Length; j++)
                return false;

        return true;
    }
}