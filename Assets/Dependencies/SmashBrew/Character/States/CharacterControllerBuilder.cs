using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HouraiTeahouse.SmashBrew.States;

namespace HouraiTeahouse.SmashBrew.Characters {

    [CreateAssetMenu]
    public partial class CharacterControllerBuilder : ScriptableObject, ISerializationCallbackReceiver {

        [Serializable]
        public class StateData {
            public string Name;
            public CharacterStateData Data;
        }

        public RuntimeAnimatorController _animatorController;
        public StateData[] _data;
        Dictionary<string, CharacterStateData> _dataMap;

        public StateControllerBuilder<CharacterState, CharacterStateContext> Builder { get; set; }

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        protected CharacterState State(string name, CharacterStateData data) {
            var state = new CharacterState(name, data);
            if (Builder != null)
                Builder.AddState(state);
            return state;
        }

        void InjectState(object obj, string path = "", int depth = 0) {
            var type = typeof(CharacterState);
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties(flags)) {
                string propertyName = propertyInfo.Name;
                if (!string.IsNullOrEmpty(path))
                    propertyName = path + "." + propertyName;
                if (propertyName == "name" || propertyName == "hideFlags" || propertyName == "Builder")
                    continue;
                var propertyType = propertyInfo.PropertyType;
                object instance;
                if (propertyType == type) {
                    if (_dataMap == null)
                        _dataMap = new Dictionary<string, CharacterStateData>();
                    if (!_dataMap.ContainsKey(propertyName))
                        _dataMap.Add(propertyName, new CharacterStateData());
                    var state = new CharacterState(propertyName, _dataMap[propertyName]);
                    Builder.AddState(state);
                    instance = state;
                } else {
                    instance = Activator.CreateInstance(propertyType);
                    if (depth < 7)
                        InjectState(instance, propertyName, depth + 1);
                }
                propertyInfo.SetValue(obj, instance, null);
            }
        }

        protected Func<CharacterStateContext, bool> Input(Func<InputContext, bool> input) {
            Argument.NotNull(input);
            return ctx => input(ctx.Input);
        }

        protected Func<CharacterStateContext, bool> Attack(Func<InputContext, bool> inputFunc = null) {
            if (inputFunc == null)
                return ctx => ctx.Input.Attack.WasPressed;
            return ctx => ctx.Input.Attack.WasPressed && inputFunc(ctx.Input);
        }

        protected Func<CharacterStateContext, bool> Special(Func<InputContext, bool> inputFunc = null) {
            if (inputFunc == null)
                return ctx => ctx.Input.Special.WasPressed;
            return ctx => ctx.Input.Special.WasPressed && inputFunc(ctx.Input);
        }

        protected Func<CharacterStateContext, bool> DirectionalInput(params Direction[] directions) {
            var set = new HashSet<Direction>(directions);
            return Input(i => set.Contains(i.Movement.Direction));
        }

        protected Func<CharacterStateContext, bool> DirectionalSmash(params Direction[] directions) {
            var set = new HashSet<Direction>(directions);
            return Input(i => set.Contains(i.Smash.Direction));
        }

        public StateController<CharacterState, CharacterStateContext> BuildCharacterControllerImpl(StateControllerBuilder<CharacterState, CharacterStateContext> builder) {
            Builder = builder;
            InjectState(this);

            // Declare Smash Attacks
            SmashUp.Charge.Data.SmashAttack = SmashAttack.Charge;
            SmashSide.Charge.Data.SmashAttack = SmashAttack.Charge;
            SmashDown.Charge.Data.SmashAttack = SmashAttack.Charge;

            SmashUp.Attack.Data.SmashAttack = SmashAttack.Attack;
            SmashSide.Attack.Data.SmashAttack = SmashAttack.Attack;
            SmashDown.Attack.Data.SmashAttack = SmashAttack.Attack;

            // Ground Attacks
            new [] {Idle, Walk, CrouchStart, Crouch, CrouchEnd}
                // Smash Attacks
                .AddTransitions<CharacterState, CharacterStateContext>(context => {
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
                .AddTransitions<CharacterState, CharacterStateContext>(context => {
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
                // Aerial Attacks
                .AddTransitions<CharacterState, CharacterStateContext>(context => {
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
            Jump.AddTransition(Idle, ctx => ctx.NormalizedAnimationTime >= 1.0f && ctx.IsGrounded)
                .AddTransition(Fall, ctx => ctx.NormalizedAnimationTime >= 1.0f && !ctx.IsGrounded);
            EscapeAir.AddTransitionTo(FallHelpless);
            new[] {Fall, FallHelpless, EscapeAir}.AddTransitions(Land, ctx => ctx.IsGrounded);
            Land.AddTransitionTo(Idle);

            // Running States
            Idle.AddTransition(Dash, DirectionalSmash(Direction.Left, Direction.Right));
            Dash.AddTransitionTo(Idle, DirectionalInput(Direction.Neutral));
            new[] {Dash, RunTurn}.AddTransitionTo(Run);
            Run.AddTransition(RunBrake, DirectionalInput(Direction.Neutral));
            Run.AddTransition(RunTurn,
                ctx => !Mathf.Approximately(Mathf.Sign(ctx.Input.Movement.Value.x), Mathf.Sign(ctx.Direction)));
            RunBrake.AddTransitionTo(Idle);

            // Ground Movement 
            new[] {Idle, Walk, Run}
                .AddTransitions(CrouchStart, DirectionalInput(Direction.Down))
                .AddTransitions(Fall, ctx => !ctx.IsGrounded);
            Idle.AddTransition(Walk, DirectionalInput(Direction.Left, Direction.Right));
            Walk.AddTransition(Idle, DirectionalInput(Direction.Neutral));

            // Crouching States
            CrouchStart.AddTransitionTo(Crouch);
            CrouchEnd.AddTransitionTo(Idle);
            new[] {CrouchStart, Crouch, CrouchEnd}.AddTransitions(Fall, ctx => !ctx.IsGrounded);
            Crouch.AddTransition(CrouchEnd, Input(i => i.Movement.Direction != Direction.Down));

            // Ledge States
            new[] {Idle, Fall, FallHelpless}.AddTransitions(LedgeGrab, ctx => ctx.IsGrabbingLedge);
            LedgeGrab.AddTransitionTo(LedgeIdle);
            LedgeIdle.AddTransition(LedgeRelease, DirectionalInput(Direction.Down))
                .AddTransition(LedgeClimb, DirectionalInput(Direction.Up))
                .AddTransition(LedgeJump, ctx => ctx.Input.Jump.WasPressed && ctx.CanJump)
                .AddTransition(LedgeAttack, Attack());
            LedgeJump.AddTransitionTo(Jump);
            new[] {LedgeRelease, LedgeClimb, LedgeEscape}
                .AddTransitions(Idle, ctx => ctx.NormalizedAnimationTime >= 1.0f && ctx.IsGrounded)
                .AddTransitions(Fall, ctx => ctx.NormalizedAnimationTime >= 1.0f && !ctx.IsGrounded);

            // Shielding
            Idle.AddTransition(Shield.On, Input(i => i.Shield.Current));
            Shield.On.AddTransition(Shield.Perfect, ctx => ctx.IsHit)
                .AddTransitionTo(Shield.Main);
            Shield.Main.AddTransition(Shield.Broken, ctx => ctx.ShieldHP <= 0)
                .AddTransition(Shield.Off, Input(i => !i.Shield.Current));
            Shield.Off.AddTransitionTo(Idle);
            new[] {Shield.Broken, Shield.Stunned, Idle}.Chain();
            
            // Rolls/Sidesteps
            Shield.Main.AddTransition(EscapeForward, DirectionalSmash(Direction.Right))
                .AddTransition(EscapeBackward, DirectionalSmash(Direction.Left))
                .AddTransition(Escape, DirectionalInput(Direction.Down));
            new[] {Escape, EscapeForward, EscapeBackward}.AddTransitionTo(Shield.Main);

            Builder.WithDefaultState(Idle);
            BuildCharacterController();
            return Builder.Build();
        }

        protected virtual void BuildCharacterController() {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (_dataMap == null)
                return;
            _data = _dataMap.Select(kvp => new StateData {Name = kvp.Key, Data = kvp.Value}).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (_data == null)
                return;
            _dataMap = _data.ToDictionary(s => s.Name, s => s.Data);
        }

    }

}
