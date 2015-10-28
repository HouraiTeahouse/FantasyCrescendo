using UnityEngine;
using UnityEngine.Networking;

namespace Hourai.SmashBrew {

    public class PlayerController : NetworkBehaviour {

        private Character _character;
        private Animator _animator;

        public ICharacterInput InputSource {
            get; set;
        }

        void Awake() {
            _character = GetComponent<Character>();
            if(!_character) {
                Destroy(this);
                return;
            }
            _animator = _character.Animator;
        }

        void FixedUpdate() {
            if(!isLocalPlayer || InputSource == null) {
                return;
            }

            Vector2 movement = InputSource.Movement;

            _animator.SetFloat(CharacterAnimVars.HorizontalInput, movement.x);
            _animator.SetFloat(CharacterAnimVars.VerticalInput, movement.y);

            //Ensure that the character is walking in the right direction
            //if ((movement.x > 0 && Facing) ||
            //    (movement.x < 0 && !Facing))
            //    Facing = !Facing;
            
            _animator.SetBool(CharacterAnimVars.JumpInput, InputSource.Jump);
            _animator.SetBool(CharacterAnimVars.AttackInput, InputSource.Attack);
            _animator.SetBool(CharacterAnimVars.SpecialInput, InputSource.Special);
            _animator.SetBool(CharacterAnimVars.ShieldInput, InputSource.Shield);
        }

    }

}
