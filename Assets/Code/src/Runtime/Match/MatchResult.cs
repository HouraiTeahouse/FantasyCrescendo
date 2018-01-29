using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

[Serializable]
public struct MatchResult {

  public MatchResolution Resolution;
  public int WinningPlayerID;
  public PlayerMatchStats[] PlayerStats;

  public override bool Equals(object obj) {
    if (typeof(MatchResult) != obj.GetType()) return false;
    var other = (MatchResult)obj;
    var equal = Resolution== other.Resolution;
    equal &= WinningPlayerID == other.WinningPlayerID;
    equal &= ArrayUtil.AreEqual(PlayerStats, other.PlayerStats);
    return equal;
  }

  public override int GetHashCode() {
    var hash = unchecked(Resolution.GetHashCode() * 31 + WinningPlayerID * 17);
    return hash + ArrayUtil.GetOrderedHash(PlayerStats);
  }

}


}
