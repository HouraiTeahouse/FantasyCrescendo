using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

/// <summary>
/// A base abstract class for assets that create runtime StateControllers
/// </summary>
public abstract class BaseStateMachineAsset : ScriptableObject {

  /// <summary>
  /// Creates a StateController based on the asset.
  /// </summary>
  /// <returns>the created StateController</returns>
  public abstract StateController BuildController();

}

}