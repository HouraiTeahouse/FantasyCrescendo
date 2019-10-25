using HouraiTeahouse.Networking;
using System;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object for configuring a single player within a multiplayer match.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 11)]
public struct PlayerConfig : IValidatable, INetworkSerializable {

  /// <summary>
  /// The Player ID of the player. Determines what is visually displayed
  /// to denote the player.
  /// </summary>
  [FieldOffset(0)]
  public byte PlayerID;

  /// <summary>
  /// The local player number. Mainly used to determine what local input 
  /// device to read the input from.
  /// </summary>
  [FieldOffset(1)]
  public sbyte LocalPlayerID;
  [FieldOffset(2)]
  public PlayerSelection Selection;

  /// <summary>
  /// The default amount of damage the player will have on (re)spawning.
  /// </summary>
  [FieldOffset(7)]
  public float DefaultDamage;

  public bool IsLocal => LocalPlayerID >= 0;
  public bool IsValid => Selection.IsValid;

  public void Serialize(ref Serializer serializer) {
    serializer.Write(PlayerID);
    serializer.Write(LocalPlayerID);
    serializer.Write(Selection);
    serializer.Write(DefaultDamage);
  }

  public void Deserialize(ref Deserializer deserializer) {
    PlayerID = deserializer.ReadByte();
    LocalPlayerID = deserializer.ReadSByte();
    Selection = deserializer.Read<PlayerSelection>();
    DefaultDamage = deserializer.ReadSingle();
  }

  public override string ToString() => Selection.ToString();

}

/// <summary>
/// A data object for managing the human selected elements of a player's
/// configuration.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 5)]
public struct PlayerSelection : IValidatable, INetworkSerializable {

  [FieldOffset(0)]
  public uint CharacterID;            // 1-4 bytes
  [FieldOffset(4)]
  public byte Pallete;                // 1 byte
  
  public bool IsValid {
    get {
      var character = Registry.Get<CharacterData>().Get(CharacterID);
      Debug.LogWarning(character);
      if (character == null) return false;
      return Pallete < character.Palletes.Length;
    }
  }

  public void Serialize(ref Serializer serializer) {
    serializer.Write(CharacterID);
    serializer.Write(Pallete);
  }

  public void Deserialize(ref Deserializer deserializer) {
    CharacterID = deserializer.ReadUInt32();
    Pallete = deserializer.ReadByte();
  }

  public CharacterData GetCharacter() => Registry.Get<CharacterData>().Get(CharacterID);
  public CharacterPallete GetPallete() => GetCharacter()?.Palletes[Pallete];

  public string GetPrettyString() => $"{GetCharacter().name}, {Pallete}";
  public override string ToString() => $"Selection({CharacterID},{Pallete})";

}

}