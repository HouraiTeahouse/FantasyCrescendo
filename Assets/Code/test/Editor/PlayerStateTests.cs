using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

public class PlayerStateTests {

	[Test]
	public void PlayerState_serializes_and_deserializes_properly() {
    for (var i = 0; i < 1000; i++) {
      var state = StateUtility.RandomPlayerState();
      var networkWriter = new NetworkWriter();
      state.Serialize(networkWriter);
      var bytes = networkWriter.AsArray();
      Debug.Log(networkWriter.Position);
      var networkReader = new NetworkReader(bytes);
      var deserializedState = new PlayerState();
      deserializedState.Deserialize(networkReader);
      Assert.AreEqual(state, deserializedState);
    }
	}

}
