using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

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

public static class ArrayUtil {

  public static bool AreEqual<T>(T[] a, T[] b) {
    if (a != null && b != null) return a.SequenceEqual(b);
    var aEmpty = a == null || a.Length == 0;
    var bEmpty = b == null || b.Length == 0;
    return aEmpty && bEmpty;
  }

  public static int GetOrderedHash<T>(T[] array) {
    int hash = array.Length;
    for (var i = 0; i < array.Length; i++) {
      hash = unchecked(hash * 17 + array[i].GetHashCode());
    }
    return hash;
  }

  public static int GetUnorderedHash<T>(T[] array) {
    int hash = array.Length;
    for (var i = 0; i < array.Length; i++) {
      hash ^= array[i].GetHashCode();
    }
    return hash;
  }
 
}

}
