using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.FantasyCrescendo.Networking;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

public class PlayerInputTests {

	[Test]
	public void PlayerInput_serializes_and_deserializes_properly() {
    var sizes = new List<int>();
    for (var i = 0; i < 10000; i++) {
      var state = InputUtility.RandomPlayerInput();
      var networkWriter = new Serializer();
      state.Serialize(networkWriter);
      var bytes = networkWriter.AsArray();
      sizes.Add(networkWriter.Position);
      var networkReader = new Deserializer(bytes);
      var deserialized = new PlayerInput();
      PlayerInput.Deserialize(networkReader, ref deserialized);
      Assert.AreEqual(state, deserialized);
    }
    Debug.Log($"Player Input: Average Message Size: {sizes.Average()}");
	}

}
