namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchPauseStateChangedEvent : MatchEvent {
  public bool IsPaused;
  public int PausedPlayerID;
}

}