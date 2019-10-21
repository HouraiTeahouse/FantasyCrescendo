using HouraiTeahouse.FantasyCrescendo;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using HouraiTeahouse.Networking;

public class PlayerInputTests {

	[Test]
	public unsafe void PlayerInput_serializes_and_deserializes_properly() {
    var sizes = new List<int>();
    var buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
    for (var i = 0; i < 10000; i++) {
      var state = InputUtility.RandomPlayerInput();
      var networkWriter = Serializer.Create(buffer, 2048);
      state.Serialize(ref networkWriter);
      var bytes = networkWriter.ToArray();
      sizes.Add(networkWriter.Position);
      fixed (byte* bytesPtr = bytes) {
        var networkReader = Deserializer.Create(bytesPtr, (uint)networkWriter.Position);
        var deserialized = new PlayerInput();
        PlayerInput.Deserialize(ref networkReader, ref deserialized);
        Assert.AreEqual(state, deserialized);
      }
    }
    Debug.Log($"Player Input: Average Message Size: {sizes.Average()}");
	}

}
