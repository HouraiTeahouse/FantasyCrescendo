using System.Linq; 

namespace HouraiTeahouse.FantasyCrescendo {

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