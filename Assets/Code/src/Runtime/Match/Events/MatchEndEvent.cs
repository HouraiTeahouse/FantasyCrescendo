namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// Called when match ends, whether it be time out or the gamemode's victory conditions are met.
/// </summary>
public class MatchEndEvent : MatchEvent {
  public MatchResult MatchResult;
}

}
