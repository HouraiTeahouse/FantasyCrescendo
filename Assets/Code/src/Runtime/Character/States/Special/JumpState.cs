using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class JumpState : CharacterState {

  CharacterMovement component;

  public override Task Initalize(GameObject gameObject, bool isView) {
    component = gameObject.GetComponentInChildren<CharacterMovement>();
    return base.Initalize(gameObject, isView);
  }

  public override void OnStateEnter(CharacterContext context) {
    base.OnStateEnter(context);
    component.Jump(ref context.State);
  }

}

}