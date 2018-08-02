using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HouraiTeahouse.FantasyCrescendo.Characters {

public partial class CharacterControllerBuilder {
  public StateController<CharacterState, CharacterContext> BuildCharacterControllerImpl(StateControllerBuilder<CharacterState, CharacterContext> builder) {
    Builder = builder;
    InjectState(this);

    Idle
      .AddTransition(SmashUp.Charge, ctx => ctx.IsGrounded);
    new[] {Walk, Run}
      .AddTransitions(SmashUp.Charge, ctx => ctx.IsGrounded)
      .AddTransitions<CharacterState, CharacterContext>(
        context => {
            var input = context.Input;
            if (!input.Attack.WasPressed)
                return null;
            switch (input.Smash.Direction) {
                case Direction.Right:
                case Direction.Left:
                    return SmashSide.Charge;
                case Direction.Up:
                    return SmashUp.Charge;
                case Direction.Down:
                    return SmashDown.Charge;
            }
            return null;
        });

    Builder.WithDefaultState(Idle);
    BuildCharacterController();
    return Builder.Build();
  }
}
}
