using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {

        private IInputController _input;

        public void SetInput(IInputController controller) {
            if (controller != null) {
                List<string> axes = controller.AxisNames.ToList();
                List<string> buttons = controller.ButtonNames.ToList();
                if(!axes.Contains("Horizontal") || !axes.Contains("Vertical") ||
                   !buttons.Contains("Jump") || !buttons.Contains("Attack") ||
                   !buttons.Contains("Special") || !buttons.Contains("Shield"))
                    throw new ArgumentException();
            }
            _input = controller;
        }        

        /// <summary>
        /// 
        /// </summary>
        void Update() {
            if (_input == null)
                return;

            Animator animator = base.Animator;

            //Ensure that the character is walking in the right direction
            //if ((movement.x > 0 && Facing) ||
            //    (movement.x < 0 && !Facing))
            animator.SetFloat(CharacterAnimVars.HorizontalInput, _input.GetAxis("Horizontal").GetAxisValue());
            animator.SetFloat(CharacterAnimVars.VerticalInput, _input.GetAxis("Vertical").GetAxisValue());
            animator.SetBool(CharacterAnimVars.JumpInput, _input.GetButton("Jump").GetButtonValue());
            animator.SetBool(CharacterAnimVars.AttackInput, _input.GetButton("Attack").GetButtonValue());
            animator.SetBool(CharacterAnimVars.SpecialInput, _input.GetButton("Special").GetButtonValue());
            animator.SetBool(CharacterAnimVars.ShieldInput, _input.GetButton("Shield").GetButtonValue());
        }

    }

}
