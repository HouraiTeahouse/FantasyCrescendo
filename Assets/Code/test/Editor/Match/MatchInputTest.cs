using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine.TestTools;

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
    src.Predict();
    Assert.IsTrue(src.IsValid);
	}

  [TestCaseSource("TestCases")]
  public void MatchInput_prodcues_proper_valid_masks(int playerCount) {
    for (var i = 0; i < 1000; i++) {
      byte mask = (byte)(Mathf.FloorToInt(Random.value) & ~(1 << playerCount));
      var input = InputUtility.RandomInput(playerCount);
      InputUtility.ForceValid(ref input, mask);
      Assert.AreEqual(mask, input.CreateValidMask());
    }
  }

}
