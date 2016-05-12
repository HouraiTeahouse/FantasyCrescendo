using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public class PlayerController : HouraiBehaviour {
        public Player PlayerData { get; set; }

        private Character _character;
        private PlayerControlMapping _controlMapping;

        private TapDetector _tap;
        private TapDetector _altTap;

        protected override void Awake() {
            base.Awake();
            _character = GetComponent<Character>();
            //TODO: Generalize this 
            _controlMapping = new PlayerControlMapping();

            _tap = new TapDetector(Config.Player.TapTreshold);
            _altTap = new TapDetector(Config.Player.TapTreshold);
        }

        void Update() {
            if (PlayerData == null || PlayerData.Controller == null || _character == null)
                return;

            InputDevice input = PlayerData.Controller;

            Vector2 stick = _controlMapping.Stick(input);
            Vector2 altStick = _controlMapping.AltStick(input);

            float dt = Time.deltaTime;
            Vector2 tap = _tap.Process(stick, dt);
            Vector2 altTap = _altTap.Process(altStick, dt);

            if (_character.Tap(tap + altTap))
                stick += altStick;
            Log.Debug(altTap);

            //Ensure that the character is walking in the right direction
            /*if (!TimeManager.Paused && (stick.x > 0 && _character.Direction) ||
                (stick.x < 0 && !_character.Direction))
                _character.Direction = !_character.Direction;*/

            _character.controlStick = stick;
            Animator.SetFloat(CharacterAnim.HorizontalInput, stick.x);
            Animator.SetFloat(CharacterAnim.VerticalInput, stick.y);
            Animator.SetBool(CharacterAnim.AttackInput, altTap.sqrMagnitude > 0 || _controlMapping.Attack(input));
            Animator.SetBool(CharacterAnim.SpecialInput, _controlMapping.Special(input));
            Animator.SetBool(CharacterAnim.ShieldInput, _controlMapping.Shield(input));

            if(_controlMapping.Jump(input))
                _character.Jump();
        }
    }

    public class TapDetector {

        private Vector2 _value;
        private Vector2 _velocity;
        private Vector2 _acceleration;

        private readonly float _deadZone;

        public TapDetector(float deadZone) {
            _deadZone = deadZone;
        }

        float Sign(float x) {
            if (x > 0)
                return 1f;
            if (x < 0)
                return -1f;
            return 0f;
        }

        Vector2 DeadZone(Vector2 v) {
            if (Mathf.Abs(v.x) < _deadZone)
                v.x = 0f;
            if (Mathf.Abs(v.y) < _deadZone)
                v.y = 0f;
            return v;
        }

        Vector2 Snap(Vector2 newV, Vector2 oldV) {
            if (Sign(newV.x) != Sign(oldV.x))
                oldV.x = 0f;
            if (Sign(newV.y) != Sign(oldV.x))
                oldV.y = 0f;
            return oldV;
        }

        Vector2 MaxComponent(Vector2 src) {
            if (Mathf.Abs(src.y) > Mathf.Abs(src.x))
                src.x = 0f;
            else
                src.y = 0f;
            return src;
        }

        public Vector2 Process(Vector2 input, float deltaT) {
            if (deltaT == 0f)
                return Vector2.zero;
            Vector2 v = input - _value;
            _acceleration = v - _velocity;
            _velocity = v;
            Log.Debug(v + " " + _velocity + " " + _acceleration);
            _value = input;
            return MaxComponent(Snap(input, _acceleration));
        }
    }
}
