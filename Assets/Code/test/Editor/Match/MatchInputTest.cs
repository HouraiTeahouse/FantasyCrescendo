using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
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
	public void Predict_forces_invalid_inputs_to_be_valid(int playerCount) {
    var src = InputUtility.RandomInput(playerCount);
    src.Predict();
    Assert.IsTrue(src.IsValid);
	}

  [Test]
  public void MergeWith_copies_valid_inputs_into_invalid_slots() {
    var match1 = new MatchInput(4);
    var match2 = new MatchInput(4);

    match1[0] = new PlayerInput();
    match1[1] = new PlayerInput { Movement = Vector2.right };
    match1[2] = new PlayerInput();
    match1[3] = new PlayerInput { Movement = Vector2.left };
    match1.ValidMask = (byte)10;

    match2[0] = new PlayerInput { Attack = true };
    match2[1] = new PlayerInput { Movement = Vector2.up };
    match2[2] = new PlayerInput { Shield = true };
    match2[3] = new PlayerInput { Smash = Vector2.up };
    match2.ValidMask = (byte)12;

    MatchInput merged = match1.MergeWith(match2);

    Assert.AreEqual(match1[0], merged[0]);
    Assert.AreEqual(match1[1], merged[1]);
    Assert.AreEqual(match2[2], merged[2]);
    Assert.AreEqual(match1[3], merged[3]);
  }

}
