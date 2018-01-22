namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class PlayerEvent : MatchEvent {

  public uint PlayerID;
  public PlayerState PlayerState {
    get { return MatchState.GetPlayerState(PlayerID); }
    set { MatchState.SetPlayerState(PlayerID, value); }
  }

  public void Copy(PlayerEvent evt) {
    base.Copy(evt as MatchEvent);
    PlayerID = evt.PlayerID;
    PlayerState = evt.PlayerState;
  }

}

}