using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class ServerStateMessage : MessageBase {

  public uint Timestamp;
  public MatchState State;

  public override void Serialize(NetworkWriter writer) {
    writer.WritePackedUInt32(Timestamp);
    writer.WritePackedUInt32((uint)State.PlayerCount);
    for (uint i = 0; i < State.PlayerCount; i++) {
      var state = State.GetPlayerState(i);
      uint mask = 0;
      if (state.Direction) mask |= 1 << 0;
      if (state.IsFastFalling) mask |= 1 << 1;
      if (state.ShieldDamage != 0) mask |= 1 << 2;
      if (state.ShieldRecoveryCooldown != 0) mask |= 1 << 3;
      if (state.GrabbedLedgeID != 0) mask |= 1 << 4;
      if (state.Stocks != 0) mask |= 1 << 5;
      if (state.Hitstun != 0) mask |= 1 << 6;
      if (state.RemainingJumps != 0) mask |= 1 << 7;
      if (state.RespawnTimeRemaining != 0) mask |= 1 << 8;

      writer.WritePackedUInt32(mask);
      writer.Write(state.Position);
      writer.Write(state.Velocity);
      writer.Write(state.Damage);
      writer.Write(state.StateHash);
      writer.WritePackedUInt32(state.StateTick);
      if (state.ShieldDamage != 0) {
        writer.WritePackedUInt32(state.ShieldDamage);
      }
      if (state.ShieldRecoveryCooldown != 0)  {
        writer.WritePackedUInt32(state.ShieldRecoveryCooldown);
      }
      if (state.GrabbedLedgeID != 0)  {
        writer.Write(state.GrabbedLedgeID);
      }
      if (state.Stocks != 0) {
        writer.Write(state.Stocks);
      }
      if (state.Hitstun != 0) {
        writer.WritePackedUInt32(state.Hitstun);
      }
      if (state.RemainingJumps != 0) {
        writer.WritePackedUInt32(state.RemainingJumps);
      }
      if (state.RespawnTimeRemaining != 0) {
        writer.WritePackedUInt32(state.RespawnTimeRemaining);
      }
    }
  }

  public override void Deserialize(NetworkReader reader) {
    Timestamp = reader.ReadPackedUInt32();
    var count = reader.ReadPackedUInt32();
    State = new MatchState((int)count);
    for (uint i = 0; i < count; i++) {
      var state = new PlayerState();
      uint mask = reader.ReadPackedUInt32();
      state.Position = reader.ReadVector2();
      state.Velocity = reader.ReadVector2();
      state.Damage = reader.ReadSingle();
      state.StateHash = reader.ReadInt32();
      state.StateTick = reader.ReadPackedUInt32();
      state.Direction = (mask & 1 << 0) != 0;
      state.IsFastFalling = (mask & 1 << 1) != 0;
      if ((mask & 1 << 2) != 0) state.ShieldDamage = reader.ReadPackedUInt32();
      if ((mask & 1 << 3) != 0) state.ShieldRecoveryCooldown = reader.ReadPackedUInt32();
      if ((mask & 1 << 4) != 0) state.GrabbedLedgeID = reader.ReadByte();
      if ((mask & 1 << 5) != 0) state.Stocks = reader.ReadSByte();
      if ((mask & 1 << 6) != 0) state.Hitstun = reader.ReadPackedUInt32();
      if ((mask & 1 << 7) != 0) state.RemainingJumps = reader.ReadPackedUInt32();
      if ((mask & 1 << 8) != 0) state.RespawnTimeRemaining = reader.ReadPackedUInt32();

      State.SetPlayerState(i, state);
    }
  }

}

}
