namespace HouraiTeahouse.FantasyCrescendo {
    
public abstract class MatchEvent {
  public MatchConfig MatchConfig;
  public MatchState MatchState;

  public void Copy(MatchEvent evt) {
    MatchConfig = evt.MatchConfig;
    MatchState = evt.MatchState;
  }
}

}