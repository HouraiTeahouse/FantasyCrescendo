using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IValidatable {
  bool IsValid { get; }
}

public static class IValidatableExtensions {

  public static bool IsAllValid<T>(this IEnumerable<T> group) where T : IValidatable {
      if (group == null) {
        return false;
      }
      return group.All(v => v.IsValid);
  }

}

}
