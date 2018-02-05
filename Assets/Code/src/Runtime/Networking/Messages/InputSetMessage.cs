using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Mask = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class InputSetMessage : INetworkSerializable, IDisposable {

  public uint StartTimestamp;
  public uint InputCount;
  public byte ValidMask;
  public MatchInput[] Inputs;

  public void Serialize(Serializer serializer) {
    Assert.IsTrue(InputCount <= Inputs.Length);
    serializer.Write(InputCount);                   // 1-4 bytes
    if (InputCount <= 0) return;
    var firstInput = Inputs[0];
    var playerCount = (byte)firstInput.PlayerCount;

    // The number of players is encoded as the N + 1 bit in the ValidMask
    // The highest bit's position represents the number of players stored
    // As the size of the mask is one byte, the maximum supported players
    // is (8 bits - 1 for count) => 7 players.
    Assert.IsTrue(playerCount < sizeof(Mask) * 8);
    ValidMask &= (byte)((1 << playerCount + 1) - 1);        // Disable all bits higher than N + 1
    ValidMask |= (byte)(1 << playerCount);                  // Set the count bit to 1.

    serializer.Write(ValidMask);                                // 1 byte
    serializer.Write(StartTimestamp);               // 1-4 bytes
    for (var i = 0; i < playerCount; i++) {
      if ((ValidMask & (1 << i)) == 0) continue;
      PlayerInput? lastInput= null;
      for (int j = 0; j < InputCount; j++) {                // 1-5 * playerCount * Inputs.Length bytes
        var currentInput = Inputs[j].PlayerInputs[i];       // (Only valid inputs)
        currentInput.Serialize(serializer, lastInput);
        lastInput = currentInput;
      }
    }
  }

  public void Deserialize(Deserializer deserializer) {
    InputCount = deserializer.ReadUInt32();
    if (InputCount <= 0) return;
    ValidMask = deserializer.ReadByte();
    StartTimestamp = deserializer.ReadUInt32();
    byte playerCount = GetPlayerCount(ValidMask);
    Inputs = ArrayPool<MatchInput>.Shared.Rent((int)InputCount);
    for (int i = 0; i < InputCount; i++) {
      Inputs[i] = new MatchInput(playerCount);
    }
    for (var i = 0; i < playerCount; i++) {
      if ((ValidMask & (1 << i)) == 0) continue;
      PlayerInput? lastInput= null;
      for (int j = 0; j < InputCount; j++) {
        var currentInput = PlayerInput.Deserialize(deserializer, lastInput);
        Inputs[j].PlayerInputs[i] = currentInput;
        lastInput = currentInput;
      }
    }
  }

  public void Dispose() {
    if (Inputs == null) return;
    ArrayPool<MatchInput>.Shared.Return(Inputs);
    Inputs = null;
  }

  byte GetPlayerCount(byte val) {
    byte bit = sizeof(Mask) * 8 - 1;
    while (bit >= 0 && (val & (1 << bit)) == 0) {
      bit--;
    }
    return bit;
  }

}

}
