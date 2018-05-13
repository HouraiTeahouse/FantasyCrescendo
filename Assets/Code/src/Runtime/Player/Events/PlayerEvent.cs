using HouraiTeahouse.FantasyCrescendo.Matches;

namespace HouraiTeahouse.FantasyCrescendo.Players {
    
public abstract class PlayerEvent : MatchEvent {

  public int PlayerID;
  public PlayerState PlayerState {
    get { return MatchState.GetPlayerState(PlayerID); }
    set { MatchState.SetPlayerState(PlayerID, value); }
  }

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

}