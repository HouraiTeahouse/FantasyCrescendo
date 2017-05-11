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
        // Jump States
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
				// Ledge States
				// -----------------------------------------------
				protected CharacterState LedgeGrab;
				protected CharacterState LedgeIdle;
				protected CharacterState LedgeClimb;
				protected CharacterState LedgeEscape;
				protected CharacterState LedgeAttack;
				protected CharacterState LedgeJump;
				protected CharacterState LedgeRelease;

				// -----------------------------------------------
				// Damaged/Launched States
				// -----------------------------------------------

				protected CharacterState DamageStun;
				protected CharacterState DamageStunAir;
				protected CharacterState DamageLaunched;
				protected CharacterState DamageFall; // tumble equivalent

				// -----------------------------------------------
				// Grabbed/Thrown States
				// -----------------------------------------------
				/*
				// grabbed
				protected CharacterState GrabbedPulled;
				protected CharacterState GrabbedWait;
				protected CharacterState GrabbedInterrupt;
				protected CharacterState GrabbedInterruptJump;
				protected CharacterState GrabbedDamaged;

				// thrown
				protected CharacterState ThrownUp;
				protected CharacterState ThrownDown;
				protected CharacterState ThrownForward;
				protected CharacterState ThrownBackward;
				*/

				// -----------------------------------------------
				// Environmental Collision States
				// -----------------------------------------------
				protected CharacterState StopCeiling;
				protected CharacterState StopWall;
				protected CharacterState Crash;

				// prone
				protected CharacterState Prone;
				protected CharacterState ProneStand;
				protected CharacterState ProneAttack;
				protected CharacterState ProneRollLeft;
				protected CharacterState ProneRollRight;

				// teching
				protected CharacterState Tech;
				protected CharacterState TechRollLeft;
				protected CharacterState TechRollRight;
				protected CharacterState TechWall;
				protected CharacterState TechWallJump;

        // -----------------------------------------------
        // Attacks
        // -----------------------------------------------
        // Neutral Combo
        protected CharacterState[] Neutral;

        // Tilt Attacks
        protected CharacterState TiltUp;
        protected CharacterState TiltSide;
        protected CharacterState TiltDown;

        // Smash Attacks
        protected CharacterState SmashUp;
        protected CharacterState SmashSide;
        protected CharacterState SmashDown;

        // Aerial Attacks
        protected CharacterState AerialNeutral;
        protected CharacterState AerialForward;
        protected CharacterState AerialBackward;
        protected CharacterState AerialUp;
        protected CharacterState AerialDown;

        // Special Attacks
        protected CharacterState SpecialNeutral;
        protected CharacterState SpecialUp;
        protected CharacterState SpecialSide;
        protected CharacterState SpecialDown;

				/*
				// Grabs
				protected CharacterState GrabPull;
				protected CharacterState GrabRunPull;
				protected CharacterState GrabWait;
				protected CharacterState GrabInterrupted;
				protected CharacterState GrabPummel;

				// Throwing
				protected CharacterState ThrowForward;
				protected CharacterState ThrowBackward;
				protected CharacterState ThrowUp;
				protected CharacterState ThrowDown;
				*/


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
