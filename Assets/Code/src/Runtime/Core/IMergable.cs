using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IMergable<T> {

  void MergeWith(T obj);

}

}