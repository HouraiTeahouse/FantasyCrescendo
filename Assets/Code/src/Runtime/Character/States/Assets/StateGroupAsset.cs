using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateGroupAsset : BaseStateAsset {

  public List<StateAsset> States;

  public override IEnumerable<StateAsset> GetBaseStates() => States;

}

}