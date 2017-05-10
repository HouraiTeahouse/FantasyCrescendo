using UnityEngine;
using HouraiTeahouse.SmashBrew.States;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class CharacterControllerBuilder : ScriptableObject {

        public DefaultCharacterStateData Data;

        // -----------------------------------------------
        // Ground Movement States
        // -----------------------------------------------
        protected CharacterState Idle;
        protected CharacterState Walk;

        // Crouch States
        protected CharacterState Crouch;
        protected CharacterState CrouchStart;
        protected CharacterState CrouchEnd;

        protected CharacterState Dash;
        protected CharacterState Run;
        protected CharacterState RunBrake;
        protected CharacterState RunTurn;

        // -----------------------------------------------
        // Shield States
        // -----------------------------------------------
        protected CharacterState Jump;
        protected CharacterState JumpStart;
        protected CharacterState JumpAerial;
        protected CharacterState Land;

        // -----------------------------------------------
        // Shield States
        // -----------------------------------------------
        protected CharacterState ShieldOn;
        protected CharacterState ShieldPerfect;
        protected CharacterState Shield;
        protected CharacterState ShieldOff;
        protected CharacterState ShieldBroken;
        protected CharacterState ShieldStunned;

        // Ground Dodge States
        protected CharacterState Escape;
        protected CharacterState EscapeForward;
        protected CharacterState EscapeBackward;

        // -----------------------------------------------
        // Air States 
        // -----------------------------------------------
        protected CharacterState Fall;
        protected CharacterState FallHelpless;
        protected CharacterState EscapeAir;

        // -----------------------------------------------
        // Attacks
        // -----------------------------------------------
        // Neutral Combo
        protected CharacterState[] Netural;

        // Tilt Attacks
        protected CharacterState TiltUp;
        protected CharacterState TiltSide;
        protected CharacterState TiltDown;

        // Smash Attacks
        protected CharacterState SmashUp;
        protected CharacterState SmashSide;
        protected CharacterState SmashDown;

        // Aerial Attacks
        protected CharacterState AerialNetural;
        protected CharacterState AerialForward;
        protected CharacterState AerialBackward;
        public CharacterStateData AerialUp;
        public CharacterStateData AerialDown;

        // Special Attacks
        public CharacterStateData SpecialUp;
        public CharacterStateData SpecialSide;
        public CharacterStateData SpecialDown;

        protected CharacterState[] GroundMovement;

        protected internal StateControllerBuilder<CharacterState, CharacterStateContext> Builder { get; internal set; }

        protected CharacterState State(string name, CharacterStateData data) {
            var state = new CharacterState(name, data);
            if (Builder != null)
                Builder.AddState(state);
            return state;
        }

        public void BuildCharacterControllerImpl() {
            Idle = State("Idle", Data.Idle);
            Walk = State("Walk", Data.Walk);
            Run = State("Run", Data.Run);

            Crouch = State("Crouch", Data.Crouch);
            CrouchStart = State("Crouch.Start", Data.CrouchStart);
            CrouchEnd = State("Crouch.End", Data.CrouchEnd);

            Builder.WithDefaultState(Idle);
            BuildCharacterController();
        }

        public virtual void BuildCharacterController() {
        }

    }

}
