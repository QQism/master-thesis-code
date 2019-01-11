using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

using HungarianAlgorith;

public class HungarianAlgorithmTests {

    [Test]
    public void HungarianAlgorithm_Tests() {
        int[, ] costs = new int[4, 4];
        costs[0, 0] = 82;
        costs[0, 1] = 83;
        costs[0, 2] = 69;
        costs[0, 3] = 92;

        costs[1, 0] = 77;
        costs[1, 1] = 37;
        costs[1, 2] = 49;
        costs[1, 3] = 92;

        costs[2, 0] = 11;
        costs[2, 1] = 69;
        costs[2, 2] = 5;
        costs[2, 3] = 86;

        costs[3, 0] = 8;
        costs[3, 1] = 9;
        costs[3, 2] = 98;
        costs[3, 3] = 23;

        int[] results = HungarianAlgorithm.FindAssignments(costs);

        Assume.That(results.Length, Is.EqualTo(4));
        Assume.That(results[0], Is.EqualTo(2));
        Assume.That(results[1], Is.EqualTo(1));
        Assume.That(results[2], Is.EqualTo(0));
        Assume.That(results[3], Is.EqualTo(3));
    }
}
