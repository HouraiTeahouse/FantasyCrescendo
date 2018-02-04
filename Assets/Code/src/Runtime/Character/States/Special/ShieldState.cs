using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class ShieldState : CharacterState {

  CharacterShield component;

  public override Task Initalize(GameObject gameObject, bool isView) {
    component = gameObject.GetComponentInChildren<CharacterShield>();
    return base.Initalize(gameObject, isView);
  }

  public override void OnStateEnter(CharacterContext context) => SetShieldActive(true);
  public override void OnStateUpdate(CharacterContext context) => SetShieldActive(true);
  public override void OnStateExit(CharacterContext context) => SetShieldActive(false);

  void SetShieldActive(bool active) {
    // TODO(james7132): Implement
  }

}

}