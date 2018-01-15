namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class PlayerEvent : GameEvent {

  public uint PlayerID;
  public PlayerState PlayerState {
    get { return GameState.PlayerStates[PlayerID]; }
    set { GameState.PlayerStates[PlayerID] = value; }
  }

  public void Copy(PlayerEvent evt) {
    base.Copy(evt as GameEvent);
    PlayerID = evt.PlayerID;
    PlayerState = evt.PlayerState;
  }

}

}