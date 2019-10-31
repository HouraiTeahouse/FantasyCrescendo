using HouraiTeahouse.FantasyCrescendo.Matches;

namespace HouraiTeahouse.FantasyCrescendo.Players {
    
public abstract class PlayerEvent : MatchEvent {

  public int PlayerID;
  public ref PlayerState PlayerState => ref MatchState[PlayerID];

  public PlayerEvent() {}

  public PlayerEvent(PlayerEvent evt) {
    Copy(evt);
  }

  public void Copy(PlayerEvent evt) {
    base.Copy(evt as MatchEvent);
    PlayerID = evt.PlayerID;
    PlayerState = evt.PlayerState;
  }

}

public class PlayerDiedEvent : PlayerEvent {
  public bool Respawned;
}
    
public class PlayerResetEvent : PlayerEvent {
  public PlayerResetEvent(PlayerEvent evt) : base(evt) {}
}

public class PlayerRespawnEvent : PlayerEvent {
  public PlayerRespawnEvent(PlayerEvent evt) : base(evt) {}
}

}