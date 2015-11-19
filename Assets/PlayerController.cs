using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {

        private Character _character;
        private Animator _animator;
        private int _playerNumber;
        private InputController _input;

        public int PlayerNumber {
            get { return _playerNumber; }
            set {
                _playerNumber = value;
                _input = InputManager.GetController(value);
            }
        }

        void Awake() {
            _character = GetComponent<Character>();
            if(!_character) {
                Destroy(this);
                return;
            }
            _animator = _character.Animator;
        }

        void Update() {
            if (_input == null)
                return;



            //Ensure that the character is walking in the right direction
            //if ((movement.x > 0 && Facing) ||
            //    (movement.x < 0 && !Facing))
            //    Facing = !Facing;

            _animator.SetFloat(CharacterAnimVars.HorizontalInput, _input.Horizontal.GetAxisValue());
            _animator.SetFloat(CharacterAnimVars.VerticalInput, _input.Vertical.GetAxisValue());
            _animator.SetBool(CharacterAnimVars.JumpInput, _input.Jump.GetButtonValue());
            _animator.SetBool(CharacterAnimVars.AttackInput, _input.Attack.GetButtonValue());
            _animator.SetBool(CharacterAnimVars.SpecialInput, _input.Special.GetButtonValue());
            _animator.SetBool(CharacterAnimVars.ShieldInput, _input.Shield.GetButtonValue());
        }

    }

}
