using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {

        public Player PlayerData { get; set; }

        void Update() {
            if (PlayerData == null || PlayerData.Controller == null)
                return;

            IInputController input = PlayerData.Controller;

            //Ensure that the character is walking in the right direction
            //if ((movement.x > 0 && Facing) ||
            //    (movement.x < 0 && !Facing))
            Animator.SetFloat(CharacterAnimVars.HorizontalInput, input.GetAxis("Horizontal").GetAxisValue());
            Animator.SetFloat(CharacterAnimVars.VerticalInput, input.GetAxis("Vertical").GetAxisValue());
            Animator.SetBool(CharacterAnimVars.JumpInput, input.GetButton("Jump").GetButtonValue());
            Animator.SetBool(CharacterAnimVars.AttackInput, input.GetButton("Attack").GetButtonValue());
            Animator.SetBool(CharacterAnimVars.SpecialInput, input.GetButton("Special").GetButtonValue());
            Animator.SetBool(CharacterAnimVars.ShieldInput, Mathf.Approximately(input.GetAxis("Shield").GetAxisValue(), 0f));
        }

    }

}
