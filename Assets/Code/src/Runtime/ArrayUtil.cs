using System.Collections.Generic;
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

  public static ArraySlice<T> GetSlice<T>(this T[] array, uint end) => new ArraySlice<T>(array, end);

  public static T[] ConvertToArray<T>(IEnumerable<T> values, out int size) {
    size = values.Count();
    T[] array = ArrayPool<T>.Shared.Rent(size);
    int index = 0;
    foreach(var val in values) {
      array[index] = val;
      index++;
    }
    return array;
  }
 
}

}