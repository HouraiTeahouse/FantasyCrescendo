using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CreateAssetMenu]
public partial class CharacterControllerBuilder : ScriptableObject, ISerializationCallbackReceiver {

  [Serializable]
  public class StateData {
    public string Name;
    public CharacterStateData Data;
  }

  public GameObject _prefab;
  public StateData[] _data;
  public CharacterStateData _default;
  Dictionary<string, CharacterStateData> _dataMap;

  public StateControllerBuilder<CharacterState, CharacterContext> Builder { get; set; }

  CharacterStateData GetStateData(string key) {
    _dataMap = _dataMap ?? new Dictionary<string, CharacterStateData>();
    CharacterStateData data;
    if (!_dataMap.TryGetValue(key, out data)) {
      data = new CharacterStateData();
      _dataMap.Add(key, data);
    }
    return data;
  }

  void InjectState(object obj, string path = "", int depth = 0) {
    uint id = 0;
    InjectState(obj, ref id, path, depth);
  }

  void InjectState(object obj, ref uint currentId, string path = "", int depth = 0) {
    const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties(flags)) {
      string propertyName = propertyInfo.Name;
      if (propertyName == "name" || propertyName == "hideFlags" || propertyName == "Builder")
        continue;
      if (!string.IsNullOrEmpty(path))
        propertyName = path + "." + propertyName;
      object instance = Activator.CreateInstance(propertyInfo.PropertyType);
      var state = instance as CharacterState;
      propertyInfo.SetValue(obj, instance, null);
      if (state != null) {
        state.Initalize(propertyName, currentId++, GetStateData(propertyName));
        Builder.AddState(state);
      } else {
        if (depth < 7) {
          InjectState(instance, ref currentId, propertyName, depth + 1);
        }
      }
    }
  }

  protected Func<CharacterContext, bool> Input(Func<PlayerInputContext, bool> input) {
    Argument.NotNull(input);
    return ctx => input(ctx.Input);
  }

  protected Func<CharacterContext, bool> Attack(Func<PlayerInputContext, bool> inputFunc = null) {
    if (inputFunc == null)
      return ctx => ctx.Input.Attack.WasPressed;
    return ctx => ctx.Input.Attack.WasPressed && inputFunc(ctx.Input);
  }

  protected Func<CharacterContext, bool> Special(Func<PlayerInputContext, bool> inputFunc = null) {
    if (inputFunc == null)
      return ctx => ctx.Input.Special.WasPressed;
    return ctx => ctx.Input.Special.WasPressed && inputFunc(ctx.Input);
  }

  protected Func<CharacterContext, bool> DirectionInput(Direction direction) {
    return Input(i => direction == i.Movement.Direction);
  }

  protected Func<CharacterContext, bool> DirectionalSmash(Direction direction) {
    return Input(i => direction == i.Smash.Direction);
  }

  public StateController<CharacterState, CharacterContext> BuildCharacterControllerImpl(StateControllerBuilder<CharacterState, CharacterContext> builder) {
    Builder = builder;
    InjectState(this);

    // Ground Attacks
    new [] {Idle, Walk, CrouchStart, Crouch, CrouchEnd}
        // Smash Attacks
        .AddTransitions<CharacterState, CharacterContext>(context => {
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
        })
        // Tilt Attacks
        .AddTransitions<CharacterState, CharacterContext>(context => {
            var input = context.Input;
            if (!input.Attack.WasPressed)
                return null;
            switch (input.Movement.Direction) {
                case Direction.Right:
                case Direction.Left:
                    return TiltSide;
                case Direction.Up:
                    return TiltUp;
                case Direction.Down:
                    return TiltDown;
            }
            return Neutral;
        });
    SmashUp.Charge.AddTransitionTo(SmashUp.Attack);
    SmashDown.Charge.AddTransitionTo(SmashDown.Attack);
    SmashSide.Charge.AddTransitionTo(SmashSide.Attack);
    TiltDown.AddTransitionTo(Crouch, Input(i => i.Movement.Direction == Direction.Down));
    new[] {Neutral, TiltUp, TiltDown, TiltSide, SmashUp.Attack, SmashDown.Attack, SmashSide.Attack}
        .AddTransitionTo(Idle);

    new [] {Fall, Jump, JumpAerial}
        .AddTransitions(Land, ctx => ctx.IsGrounded)
        // Aerial Attacks
        .AddTransitions<CharacterState, CharacterContext>(context => {
            var input = context.Input;
            if (!input.Attack.WasPressed)
                return null;
            switch (input.Movement.Direction) {
                case Direction.Right:
                    return context.Direction >= 0f ? AerialForward : AerialBackward;
                case Direction.Left:
                    return context.Direction >= 0f ? AerialBackward : AerialForward;
                case Direction.Up:
                    return AerialUp;
                case Direction.Down:
                    return AerialDown;
            }
            return AerialNeutral;
        });
    new[] {AerialForward, AerialBackward, AerialDown, AerialUp, AerialNeutral}
        .AddTransitions(AerialAttackLand, ctx => ctx.IsGrounded)
        .AddTransitionTo(Fall);
    AerialAttackLand.AddTransitionTo(Idle);

    // Aerial Movement
    new [] {Idle, Walk, Dash, Run, RunTurn, RunBrake, CrouchStart, Crouch, CrouchEnd, Shield.Main} 
        .AddTransitions(JumpStart, ctx => ctx.Input.Jump.WasPressed && ctx.CanJump);
    new[] {JumpStart, JumpAerial}.AddTransitionTo(Jump);
    new[] {Jump, Fall}.AddTransitions(JumpAerial, ctx => ctx.Input.Jump.WasPressed && ctx.CanJump)
                      .AddTransitions(EscapeAir, Input(i => i.Shield.WasPressed));
    Jump.AddTransition(Idle, ctx => ctx.NormalizedStateTime >= 1.0f && ctx.IsGrounded)
        .AddTransition(Fall, ctx => ctx.NormalizedStateTime >= 1.0f && !ctx.IsGrounded);
    EscapeAir.AddTransitionTo(FallHelpless);
    new[] {Fall, FallHelpless, EscapeAir}.AddTransitions(Land, ctx => ctx.IsGrounded);
    Land.AddTransitionTo(Idle);

    Func<Func<PlayerInputContext, DirectionalInput>, Func<CharacterContext, bool>>
        movementContext = func => {
          var downMove = DirectionInput(Direction.Down);
          var lateralMovement = Input(i => Mathf.Abs(func(i).Value.x) > DirectionalInput.DeadZone);
          return ctx => !downMove(ctx) && lateralMovement(ctx);
        };

    // Running States
    Idle.AddTransition(Dash, movementContext(i => i.Smash));
    Dash.AddTransitionTo(Idle, DirectionInput(Direction.Neutral));
    new[] {Dash, RunTurn}.AddTransitionTo(Run);
    Run.AddTransition(RunBrake, DirectionInput(Direction.Neutral));
    Run.AddTransition(RunTurn,
        ctx => !Mathf.Approximately(Mathf.Sign(ctx.Input.Movement.Value.x), Mathf.Sign(ctx.Direction)));
    RunBrake.AddTransitionTo(Idle);

    // Ground Movement 
    new[] {Idle, Walk, Run}
        .AddTransitions(CrouchStart, DirectionInput(Direction.Down))
        .AddTransitions(Fall, ctx => !ctx.IsGrounded);

    Idle.AddTransition(Walk, movementContext(i => i.Movement));
    Walk.AddTransition(Idle, DirectionInput(Direction.Neutral));

    // Crouching States
    CrouchStart.AddTransitionTo(Crouch);
    CrouchEnd.AddTransitionTo(Idle);
    new[] {CrouchStart, Crouch, CrouchEnd}.AddTransitions(Fall, ctx => !ctx.IsGrounded);
    Crouch.AddTransition(CrouchEnd, Input(i => i.Movement.Direction != Direction.Down));

    // Ledge States
    new[] {Idle, Fall, FallHelpless}.AddTransitions(LedgeGrab, ctx => ctx.State.IsGrabbingLedge);
    LedgeGrab.AddTransitionTo(LedgeIdle);
    LedgeIdle.AddTransition(LedgeRelease, ctx => !ctx.State.IsGrabbingLedge)
        .AddTransition(LedgeClimb, DirectionInput(Direction.Up))
        .AddTransition(LedgeJump, ctx => ctx.Input.Jump.WasPressed && ctx.CanJump)
        .AddTransition(LedgeAttack, Attack());
    LedgeJump.AddTransitionTo(Jump);
    new[] {LedgeRelease, LedgeClimb, LedgeEscape, LedgeAttack}
        .AddTransitions(Idle, ctx => ctx.NormalizedStateTime >= 1.0f && ctx.IsGrounded)
        .AddTransitions(Fall, ctx => ctx.NormalizedStateTime >= 1.0f && !ctx.IsGrounded);

    // Shielding
    Idle.AddTransition(Shield.On, Input(i => i.Shield.Current));
    Shield.On.AddTransition(Shield.Perfect, ctx => ctx.State.IsHit)
        .AddTransitionTo(Shield.Main);
    Shield.Main.AddTransition(Shield.Broken, ctx => ctx.State.ShieldDamage <= 0)
        .AddTransition(Shield.Off, Input(i => !i.Shield.Current));
    Shield.Off.AddTransitionTo(Idle);
    new[] {Shield.Broken, Shield.Stunned, Idle}.Chain();
    
    // Rolls/Sidesteps
    var leftSmash = DirectionalSmash(Direction.Left);
    var rightSmash = DirectionalSmash(Direction.Right);
    Shield.Main
    .AddTransition(EscapeForward, ctx => {
        if (ctx.Direction > 0f)
            return rightSmash(ctx);
        else
            return leftSmash(ctx);
    })
    .AddTransition(EscapeBackward, ctx => {
        if (ctx.Direction > 0f)
            return leftSmash(ctx);
        else
            return rightSmash(ctx);
        })
    .AddTransition(Escape, DirectionInput(Direction.Down));
    new[] {Escape, EscapeForward, EscapeBackward}.AddTransitionTo(Shield.Main);

    Builder.WithDefaultState(Idle);
    BuildCharacterController();
    return Builder.Build();
}

  protected virtual void BuildCharacterController() {
  }

  void ISerializationCallbackReceiver.OnBeforeSerialize() {
    if (_dataMap == null) return;
    _data = _dataMap.Select(kvp => new StateData {Name = kvp.Key, Data = kvp.Value}).ToArray();
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize() {
    if (_data == null) return;
    _dataMap = _data.ToDictionary(s => s.Name, s => s.Data);
  }

}

}
