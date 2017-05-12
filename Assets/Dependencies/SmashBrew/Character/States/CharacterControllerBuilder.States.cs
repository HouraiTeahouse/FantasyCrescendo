namespace HouraiTeahouse.SmashBrew.Characters {

    public partial class CharacterControllerBuilder {

        // -----------------------------------------------
        // Ground Movement States
        // -----------------------------------------------
        protected CharacterState Idle { get; private set; }
        protected CharacterState Walk { get; private set; }

        // Crouch States
        protected CharacterState Crouch { get; private set; }
        protected CharacterState CrouchStart { get; private set; }
        protected CharacterState CrouchEnd { get; private set; }

        protected CharacterState Dash { get; private set; }
        protected CharacterState Run { get; private set; }
        protected CharacterState RunBrake { get; private set; }
        protected CharacterState RunTurn { get; private set; }

        // -----------------------------------------------
        // Jump States
        // -----------------------------------------------
        protected CharacterState Jump { get; private set; }
        protected CharacterState JumpStart { get; private set; }
        protected CharacterState JumpAerial { get; private set; }
        protected CharacterState Land { get; private set; }

        // -----------------------------------------------
        // Shield States
        // -----------------------------------------------
        protected class ShieldStates {
            public CharacterState On { get; set; }
            public CharacterState Perfect { get; set; }
            public CharacterState Main { get; set; }
            public CharacterState Off { get; set; }
            public CharacterState Broken { get; set; }
            public CharacterState Stunned { get; set; }
        }

        protected class SmashAttack {
            public CharacterState Charge { get; set; }
            public CharacterState Attack { get; set; }
        }

        protected ShieldStates Shield { get; private set; }

        // Ground Dodge States
        protected CharacterState Escape { get; private set; }
        protected CharacterState EscapeForward { get; private set; }
        protected CharacterState EscapeBackward { get; private set; }

        // -----------------------------------------------
        // Air States
        // -----------------------------------------------
        protected CharacterState Fall { get; private set; }
        protected CharacterState FallHelpless { get; private set; }
        protected CharacterState EscapeAir { get; private set; }

        // -----------------------------------------------
        // Ledge States
        // -----------------------------------------------
        protected CharacterState LedgeGrab { get; private set; }
        protected CharacterState LedgeIdle { get; private set; }
        protected CharacterState LedgeClimb { get; private set; }
        protected CharacterState LedgeEscape { get; private set; }
        protected CharacterState LedgeAttack { get; private set; }
        protected CharacterState LedgeJump { get; private set; }
        protected CharacterState LedgeRelease { get; private set; }

        // -----------------------------------------------
        // Damaged/Launched States
        // -----------------------------------------------
        protected CharacterState DamageStun { get; private set; }
        protected CharacterState DamageStunAir { get; private set; }
        protected CharacterState DamageLaunched { get; private set; }
        protected CharacterState DamageFall { get; private set; } // tumble equivalent

        // -----------------------------------------------
        // Grabbed/Thrown States
        // -----------------------------------------------
        /*
        // grabbed
        protected CharacterState GrabbedPulled { get; private set; }
        protected CharacterState GrabbedWait { get; private set; }
        protected CharacterState GrabbedInterrupt { get; private set; }
        protected CharacterState GrabbedInterruptJump { get; private set; }
        protected CharacterState GrabbedDamaged { get; private set; }

        // thrown
        protected CharacterState ThrownUp { get; private set; }
        protected CharacterState ThrownDown { get; private set; }
        protected CharacterState ThrownForward { get; private set; }
        protected CharacterState ThrownBackward { get; private set; }
        */

        // -----------------------------------------------
        // Environmental Collision States
        // -----------------------------------------------
        protected CharacterState StopCeiling { get; private set; }
        protected CharacterState StopWall { get; private set; }
        protected CharacterState Crash { get; private set; }

        // prone
        protected CharacterState Prone { get; private set; }
        protected CharacterState ProneStand { get; private set; }
        protected CharacterState ProneAttack { get; private set; }
        protected CharacterState ProneRollLeft { get; private set; }
        protected CharacterState ProneRollRight { get; private set; }

        // teching
        protected CharacterState Tech { get; private set; }
        protected CharacterState TechRollLeft { get; private set; }
        protected CharacterState TechRollRight { get; private set; }
        protected CharacterState TechWall { get; private set; }
        protected CharacterState TechWallJump { get; private set; }

        // -----------------------------------------------
        // Attacks
        // -----------------------------------------------
        // Neutral Combo
        protected CharacterState Neutral { get; private set; }

        // Tilt Attacks
        protected CharacterState TiltUp { get; private set; }
        protected CharacterState TiltSide { get; private set; }
        protected CharacterState TiltDown { get; private set; }

        // Smash Attacks
        protected SmashAttack SmashUp { get; private set; }
        protected SmashAttack SmashSide { get; private set; }
        protected SmashAttack SmashDown { get; private set; }

        // Aerial Attacks
        protected CharacterState AerialNeutral { get; private set; }
        protected CharacterState AerialForward { get; private set; }
        protected CharacterState AerialBackward { get; private set; }
        protected CharacterState AerialUp { get; private set; }
        protected CharacterState AerialDown { get; private set; }

        // Special Attacks
        protected CharacterState SpecialNeutral { get; private set; }
        protected CharacterState SpecialUp { get; private set; }
        protected CharacterState SpecialSide { get; private set; }
        protected CharacterState SpecialDown { get; private set; }

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

    }

}
