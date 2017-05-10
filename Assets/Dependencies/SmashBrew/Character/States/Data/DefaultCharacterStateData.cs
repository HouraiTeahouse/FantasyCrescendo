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
        // Jump States
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
        public CharacterStateData SpecialNeutral;
        public CharacterStateData SpecialUp;
        public CharacterStateData SpecialSide;
        public CharacterStateData SpecialDown;
    }

}
