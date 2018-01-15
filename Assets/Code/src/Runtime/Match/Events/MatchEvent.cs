namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class GameEvent {
  public MatchState GameState;

  public void Copy(GameEvent evt) {
    GameState = evt.GameState;
  }
}

}