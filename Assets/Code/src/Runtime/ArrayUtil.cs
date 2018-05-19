using System;
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

  public static int RemoveDuplicates<T>(T[] array) {
    int writeIndex = 0;
    for (var i = 0; i < array.Length; i++) {
      bool duplicate = false;
      for (var j = 0; j < writeIndex; j++) {
        if (array[i].Equals(array[j])) {
          duplicate = true;
          break;
        }
      }
      if (!duplicate) {
        array[writeIndex++] = array[i];
      }
    }
    for (var i = 0; i < writeIndex; i++) {
      array[i] = default(T);
    }
    return writeIndex;
  }

  public static int Join<T>(T[] lhs, T[] rhs, int lhsSize) {
    if (lhsSize >= lhs.Length) return lhs.Length;
    int rhsSize = Math.Min(rhs.Length, lhs.Length - lhsSize);
    Array.Copy(rhs, 0, lhs, lhsSize, rhsSize);
    return lhsSize + rhsSize;
  }

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