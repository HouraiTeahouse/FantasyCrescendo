using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class SmashChargeState : State {

  public override void OnStateEnter(CharacterContext context) {
    context.State.Charge = 0;
  }

  public override void OnStateUpdate(CharacterContext context) {
    var charge = context.State.Charge;
    if (charge == byte.MaxValue) return;
    context.State.Charge++;
  }

}

}