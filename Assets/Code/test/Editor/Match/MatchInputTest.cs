using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class MatchInputTest {

	[TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
	public void Clone_produces_equal_inputs(int playerCount) {
    var matchInput = InputUtility.RandomInput(playerCount);
    var clone = matchInput.Clone();
    Assert.AreEqual(matchInput, clone);
	}

	[TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
	public void Clone_produces_different_backing_arrays(int playerCount) {
    var matchInput = InputUtility.RandomInput(playerCount);
    var clone = matchInput.Clone();
    Assert.AreNotSame(matchInput.PlayerInputs, clone.PlayerInputs);
	}

}
