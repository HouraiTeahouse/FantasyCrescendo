using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.Networking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine;

public class PlayerStateTests {

	[Test]
	public unsafe void PlayerState_serializes_and_deserializes_properly() {
        var sizes = new List<int>();
        Span<byte> buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
        for (var i = 0; i < 1000; i++) {
            var state = StateUtility.RandomPlayerState();
            var networkWriter = Serializer.Create(buffer);
            state.Serialize(ref networkWriter);
            sizes.Add(networkWriter.Position);
            var networkReader = Deserializer.Create(networkWriter);
            var deserializedState = new PlayerState();
            deserializedState.Deserialize(ref networkReader);
            Assert.AreEqual(state, deserializedState);
        }
        Debug.Log($"Player State: Average Message Size: {sizes.Average()}");
	}

}
