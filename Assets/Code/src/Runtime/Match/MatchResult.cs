using System;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct MatchResult {
  public MatchResolution Resolution;
  public int WinningPlayerID;
  public PlayerMatchStats[] PlayerStats;
}

}
