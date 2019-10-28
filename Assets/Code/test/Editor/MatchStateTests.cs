using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.Networking;

public class MatchStateTests {

  static IEnumerable<object[]> TestCases() {
    for (var i = 1; i <= GameMode.GlobalMaxPlayers; i++) {
      yield return new object[] { i };
    }
  }

	[TestCaseSource("TestCases")]
	public unsafe void MatchInput_serializes_and_deserializes_properly(int playerCount) {
    var sizes = new List<int>();
    var buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
    for (var i = 0; i < 1000; i++) {
      var input = StateUtility.RandomState(playerCount);
      var networkWriter = Serializer.Create(buffer, 2048);
      input.Serialize(ref networkWriter);
      sizes.Add(networkWriter.Position);
      var networkReader = Deserializer.Create(networkWriter.ToFixedBuffer());
      var deserialized = new MatchState(playerCount);
      deserialized.Deserialize(ref networkReader);
      Assert.AreEqual(input, deserialized);
    }
    Debug.Log($"Match Input: Average Message Size ({playerCount}): {sizes.Average()}");
	}

}
