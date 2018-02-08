using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class SmashAttackState : CharacterState {

  public override void OnStateExit(CharacterContext context) {
    context.State.Charge = 0;
  }

}

}