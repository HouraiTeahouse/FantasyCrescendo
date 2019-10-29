using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class JumpState : State {

  CharacterMovement component;

  public override Task Initalize(Character character) {
    component = character._movement;
    return Task.CompletedTask;
  }

  public override void OnStateEnter(CharacterContext context) {
    base.OnStateEnter(context);
    if (component != null) {
      component.Jump(ref context.State);
      Debug.Log(context.State.JumpCount);
    }
  }

}

}