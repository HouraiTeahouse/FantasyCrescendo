using HouraiTeahouse.FantasyCrescendo.Players;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.Networking;

public class PlayerInputTests {

	[Test]
	public void PlayerInput_serializes_and_deserializes_properly() {
    for (var i = 0; i < 1000; i++) {
      var input = InputUtility.RandomPlayerInput();
      var networkWriter = new NetworkWriter();
      input.Serialize(networkWriter);
      var networkReader = new NetworkReader(networkWriter.AsArray());
      var deserializedInput = PlayerInput.Deserialize(networkReader);
      Assert.AreEqual(input, deserializedInput);
    }
	}

}
