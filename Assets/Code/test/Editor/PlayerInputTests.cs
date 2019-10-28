using HouraiTeahouse.FantasyCrescendo;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse {

public class PlayerInputTests {

	[Test]
	public unsafe void PlayerInput_serializes_and_deserializes_properly() {
    var sizes = new List<int>();
    var buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
    for (var i = 0; i < 10000; i++) {
      var state = InputUtility.RandomPlayerInput();
      var networkWriter = Serializer.Create(buffer, 2048);
      sizes.Add(networkWriter.Position);
      var networkReader = Deserializer.Create(networkWriter.ToFixedBuffer());
      var deserialized = new PlayerInput();
      Assert.AreEqual(state, deserialized);
    }
    Debug.Log($"Player Input: Average Message Size: {sizes.Average()}");
	}

}

}