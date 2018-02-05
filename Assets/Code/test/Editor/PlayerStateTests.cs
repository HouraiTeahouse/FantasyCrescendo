using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.FantasyCrescendo.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class PlayerStateTests {

	[Test]
	public void PlayerState_serializes_and_deserializes_properly() {
    var sizes = new List<int>();
    for (var i = 0; i < 1000; i++) {
      var state = StateUtility.RandomPlayerState();
      var networkWriter = new Serializer();
      state.Serialize(networkWriter);
      var bytes = networkWriter.AsArray();
      sizes.Add(networkWriter.Position);
      var networkReader = new Deserializer(bytes);
      var deserializedState = new PlayerState();
      deserializedState.Deserialize(networkReader);
      Assert.AreEqual(state, deserializedState);
    }
    Debug.Log($"Message Size: {sizes.Average()}");
	}

}
