using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {
        public Player PlayerData { get; set; }

        private Character _character;

        protected override void Awake() {
            base.Awake();
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

            Animator.SetFloat(CharacterAnim.HorizontalInput, input.LeftStickX.Value);
            Animator.SetFloat(CharacterAnim.VerticalInput, input.LeftStickY.Value);
            Animator.SetBool(CharacterAnim.AttackInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnim.SpecialInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnim.JumpInput, input.Action3.WasPressed || input.Action4.WasPressed);
            Animator.SetBool(CharacterAnim.ShieldInput, input.LeftTrigger || input.RightTrigger);
        }
    }
}
