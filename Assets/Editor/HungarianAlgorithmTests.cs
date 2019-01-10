using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class HungarianAlgorithmTests {

    [Test]
    public void HungarianAlgorithmTestsSimplePasses() {
        // Use the Assert class to test conditions.
		List<GameObject> bars = new List<GameObject>();
        GameObject bar1 = new GameObject("Bar1");
        GameObject bar2 = new GameObject("Bar2");
        GameObject bar3 = new GameObject("Bar3");
        GameObject bar4 = new GameObject("Bar4");
        bar1.transform.position = Vector3.zero;
        bar2.transform.position = new Vector3(0, 0, 0);

        bars.Add(bar1);
        bars.Add(bar2);
        bars.Add(bar3);
        bars.Add(bar4);

        bar1.transform.position = new Vector3(0, 0, 0);

		List<MapDataPoint> dataPoints = new List<MapDataPoint>();
        MapDataPoint point1 = new MapDataPoint();
        MapDataPoint point2 = new MapDataPoint();
        MapDataPoint point3 = new MapDataPoint();
        MapDataPoint point4 = new MapDataPoint();

        dataPoints.Add(point1);
        dataPoints.Add(point2);
        dataPoints.Add(point3);
        dataPoints.Add(point4);

        point1.WorldPosition = new Vector3(82, 0, 0);
        point2.WorldPosition = new Vector3(83, 0, 0);
        point3.WorldPosition = new Vector3(69, 0, 0);
        point4.WorldPosition = new Vector3(92, 0, 0);

		HungarianAlgorithm.Allocate(bars, dataPoints);
		Assume.That(1, Is.EqualTo(2));
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator HungarianAlgorithmTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
