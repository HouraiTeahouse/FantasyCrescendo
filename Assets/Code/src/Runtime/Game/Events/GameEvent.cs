namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class GameEvent {
  public GameState GameState;

  public void Copy(GameEvent evt) {
    GameState = evt.GameState;
  }
}

}