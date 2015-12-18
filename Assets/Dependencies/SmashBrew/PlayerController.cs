using InControl;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {

        public Player PlayerData { get; set; }

        void Update() {
            if (PlayerData == null || PlayerData.Controller == null)
                return;

            InputDevice input = PlayerData.Controller;

            //Ensure that the character is walking in the right direction
            //if ((movement.x > 0 && Facing) ||
            //    (movement.x < 0 && !Facing))
            Animator.SetFloat(CharacterAnimVars.HorizontalInput, input.LeftStickX.Value);
            Animator.SetFloat(CharacterAnimVars.VerticalInput, input.LeftStickY.Value);
            Animator.SetBool(CharacterAnimVars.AttackInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnimVars.SpecialInput, input.Action2.WasPressed);
            Animator.SetBool(CharacterAnimVars.JumpInput, input.Action3.WasPressed || input.Action4.WasPressed);
            Animator.SetBool(CharacterAnimVars.ShieldInput, input.LeftTrigger || input.RightTrigger);
        }

    }

}
