namespace HouraiTeahouse.FantasyCrescendo.Matches {

public abstract class MatchEvent {
  public MatchConfig MatchConfig;
  public MatchState MatchState;

  public void Copy(MatchEvent evt) {
    MatchConfig = evt.MatchConfig;
    MatchState = evt.MatchState;
  }
}

/// <summary>
/// Called on the match's countdown sequence. Before the game begins.
/// </summary>
public class MatchStartCountdownEvent : MatchEvent{
}

public class MatchStartEvent : MatchEvent {
}

/// <summary>
/// Called when match ends, whether it be time out or the gamemode's victory conditions are met.
/// </summary>
public class MatchEndEvent : MatchEvent {
  public MatchResult MatchResult;
}

public class MatchPauseStateChangedEvent : MatchEvent {
  public bool IsPaused;
  public int PausedPlayerID;
}

}
