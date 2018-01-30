using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class MatchInputTest {

  static IEnumerable<object[]> TestCases() {
    for (var i = 1; i <= GameMode.GlobalMaxPlayers; i++) {
      yield return new object[] { i };
    }
  }

	[TestCaseSource("TestCases")]
	public void Clone_produces_equal_inputs(int playerCount) {
    var matchInput = InputUtility.RandomInput(playerCount);
    var clone = matchInput.Clone();
    Assert.AreEqual(matchInput, clone);
	}

	[TestCaseSource("TestCases")]
	public void Clone_produces_different_backing_arrays(int playerCount) {
    var matchInput = InputUtility.RandomInput(playerCount);
    var clone = matchInput.Clone();
    Assert.AreNotSame(matchInput.PlayerInputs, clone.PlayerInputs);
	}

	[TestCaseSource("TestCases")]
	public void Predict_forces_invalid_inputs_to_be_valid(int playerCount) {
    var src = InputUtility.RandomInput(playerCount);
    src.Predict(InputUtility.RandomInput(playerCount));
    Assert.IsTrue(src.IsValid);
	}

}
