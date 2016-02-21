using InControl;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {

        public Player PlayerData { get; set; }

        private Character _character;

        void Awake() {
            _character = GetComponent<Character>();
        }

        void Update() {
            if (PlayerData == null || PlayerData.Controller == null || _character == null)
                return;

            InputDevice input = PlayerData.Controller;

            //Ensure that the character is walking in the right direction
            if ((input.LeftStickX > 0 && _character.Direction) ||
                (input.LeftStickX < 0 && !_character.Direction))
                _character.Direction = !_character.Direction;

            Animator.SetFloat(CharacterAnimVars.HorizontalInput, input.LeftStickX.Value);
            Animator.SetFloat(CharacterAnimVars.VerticalInput, input.LeftStickY.Value);
            Animator.SetBool(CharacterAnimVars.AttackInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnimVars.SpecialInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnimVars.JumpInput, input.Action3.WasPressed || input.Action4.WasPressed);
            Animator.SetBool(CharacterAnimVars.ShieldInput, input.LeftTrigger || input.RightTrigger);
        }

    }

}
