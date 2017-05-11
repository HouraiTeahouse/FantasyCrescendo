using System;

namespace HouraiTeahouse.SmashBrew.Characters {

    [Serializable]
    public class DefaultCharacterStateData {
        // -----------------------------------------------
        // Ground Movement States
        // -----------------------------------------------
        public CharacterStateData Idle;
        public CharacterStateData Walk;

        // Crouch States
        public CharacterStateData Crouch;
        public CharacterStateData CrouchStart;
        public CharacterStateData CrouchEnd;

        public CharacterStateData Dash;
        public CharacterStateData Run;
        public CharacterStateData RunBrake;
        public CharacterStateData RunTurn;

        // -----------------------------------------------
        // Shield States
        // -----------------------------------------------
        public CharacterStateData Jump;
        public CharacterStateData JumpStart;
        public CharacterStateData JumpAerial;
        public CharacterStateData Land;

        // -----------------------------------------------
        // Shield States
        // -----------------------------------------------
        public CharacterStateData ShieldOn;
        public CharacterStateData ShieldPerfect;
        public CharacterStateData Shield;
        public CharacterStateData ShieldOff;
        public CharacterStateData ShieldBroken;
        public CharacterStateData ShieldStunned;

        // Ground Dodge States
        public CharacterStateData Escape;
        public CharacterStateData EscapeForward;
        public CharacterStateData EscapeBackward;

        // -----------------------------------------------
        // Air States
        // -----------------------------------------------
        public CharacterStateData Fall;
        public CharacterStateData FallHelpless;
        public CharacterStateData EscapeAir;

				// -----------------------------------------------
				// Damaged/Launched States
				// -----------------------------------------------

				public CharacterStateData DamageStun;
				public CharacterStateData DamageStunAir;
				public CharacterStateData DamageLaunched;
				public CharacterStateData DamageFall; // tumble equivalent

				// -----------------------------------------------
				// Grabbed/Thrown States
				// -----------------------------------------------
				/*
				// grabbed
				public CharacterStateData GrabbedPulled;
				public CharacterStateData GrabbedWait;
				public CharacterStateData GrabbedInterrupt;
				public CharacterStateData GrabbedInterruptJump;
				public CharacterStateData GrabbedDamaged;

				// thrown
				public CharacterStateData ThrownUp;
				public CharacterStateData ThrownDown;
				public CharacterStateData ThrownForward;
				public CharacterStateData ThrownBackward;
				*/

				// -----------------------------------------------
				// Environmental Collision States
				// -----------------------------------------------
				public CharacterStateData StopCeiling;
				public CharacterStateData StopWall;
				public CharacterStateData Crash;

				// prone
				public CharacterStateData Prone;
				public CharacterStateData ProneStand;
				public CharacterStateData ProneAttack;
				public CharacterStateData ProneRollLeft;
				public CharacterStateData ProneRollRight;

				// teching
				public CharacterStateData Tech;
				public CharacterStateData TechRollLeft;
				public CharacterStateData TechRollRight;
				public CharacterStateData TechWall;
				public CharacterStateData TechWallJump;

        // -----------------------------------------------
        // Attacks
        // -----------------------------------------------
        // Neutral Combo
        public CharacterStateData[] Neutral;

        // Tilt Attacks
        public CharacterStateData TiltUp;
        public CharacterStateData TiltSide;
        public CharacterStateData TiltDown;

        // Smash Attacks
        public SmashAttackStateData SmashUp;
        public SmashAttackStateData SmashSide;
        public SmashAttackStateData SmashDown;

        // Aerial Attacks
        public CharacterStateData AerialNeutral;
        public CharacterStateData AerialForward;
        public CharacterStateData AerialBackward;
        public CharacterStateData AerialUp;
        public CharacterStateData AerialDown;

        // Special Attacks
        public CharacterStateData SpecialUp;
        public CharacterStateData SpecialSide;
        public CharacterStateData SpecialDown;

				/*
				// Grab
				public CharacterStateData GrabPull;
				public CharacterStateData GrabRunPull;
				public CharacterStateData GrabWait;
				public CharacterStateData GrabInterrupted;
				public CharacterStateData GrabPummel;

				// Throwing
				public CharacterStateData ThrowForward;
				public CharacterStateData ThrowBackward;
				public CharacterStateData ThrowUp;
				public CharacterStateData ThrowDown;
				*/
    }

}
