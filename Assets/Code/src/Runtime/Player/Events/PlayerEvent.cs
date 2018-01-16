namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class PlayerEvent : MatchEvent {

  public uint PlayerID;
  public PlayerState PlayerState {
    get { return MatchState.PlayerStates[PlayerID]; }
    set { MatchState.PlayerStates[PlayerID] = value; }
  }

  public void Copy(PlayerEvent evt) {
    base.Copy(evt as MatchEvent);
    PlayerID = evt.PlayerID;
    PlayerState = evt.PlayerState;
  }

}

}