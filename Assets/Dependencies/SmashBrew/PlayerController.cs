using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {
        public Player PlayerData { get; set; }

        private Character _character;
        private PlayerControlMapping _controlMapping;

        protected override void Awake() {
            base.Awake();
            _character = GetComponent<Character>();
            //TODO: Generalize this 
            _controlMapping = new PlayerControlMapping();
        }

        void Update() {
            if (PlayerData == null || PlayerData.Controller == null || _character == null)
                return;

            InputDevice input = PlayerData.Controller;

            Vector2 stick = _controlMapping.Stick(input);
            Vector2 altStick = _controlMapping.AltStick(input);

            //Ensure that the character is walking in the right direction
            if ((stick.x > 0 && _character.Direction) ||
                (stick.x < 0 && !_character.Direction))
                _character.Direction = !_character.Direction;

            Animator.SetFloat(CharacterAnim.HorizontalInput, stick.x);
            Animator.SetFloat(CharacterAnim.VerticalInput, stick.y);
            Animator.SetBool(CharacterAnim.AttackInput, _controlMapping.Attack(input));
            Animator.SetBool(CharacterAnim.SpecialInput, _controlMapping.Special(input));
            Animator.SetBool(CharacterAnim.ShieldInput, _controlMapping.Shield(input));

            if(_controlMapping.Jump(input))
                _character.Jump();
        }
    }

}
