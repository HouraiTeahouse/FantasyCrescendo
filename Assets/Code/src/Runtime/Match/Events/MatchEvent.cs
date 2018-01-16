namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class MatchEvent {
  public MatchState MatchState;

  public void Copy(MatchEvent evt) {
    MatchState = evt.MatchState;
  }
}

}