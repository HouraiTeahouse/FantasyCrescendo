using HouraiTeahouse.HouraiInput;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    public class InputState : CharacterComponent, IDataComponent<Player> {

        PlayerControlMapping _controlMapping;

        Player Player { get; set; }

        public Vector2 Movement {
            get { 
                var move = IsInvalid ? Vector2.zero : _controlMapping.Stick(Player.Controller);
                //TODO(james7132): Turn this into an input device
                move.x += ButtonAxis(GetKeys(KeyCode.A, KeyCode.LeftArrow),
                                     GetKeys(KeyCode.D, KeyCode.RightArrow));
                move.y += ButtonAxis(GetKeys(KeyCode.S, KeyCode.DownArrow),
                                     GetKeys(KeyCode.W, KeyCode.UpArrow));
                return DirectionClamp(move);
            }
        }

        public Vector2 Smash {
            get { 
                // TODO(james7132): Do proper smash input detection
                var smash = Vector2.zero;
                smash.x += ButtonAxis(GetKeys(KeyCode.A), GetKeys(KeyCode.D));
                smash.y += ButtonAxis(GetKeys(KeyCode.W), GetKeys(KeyCode.S));
                return DirectionClamp(smash);
            }
        }

        public bool Jump {
            get {
                var val = !IsInvalid && _controlMapping.Jump(Player.Controller);
                return val || GetKeysDown(KeyCode.W, KeyCode.UpArrow);
            }
        }

        bool IsInvalid {
            get { return Player == null || Player.Controller == null; }
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            InputContext input = context.Input;
            input.Movement = Movement;
            input.Smash = Smash;
            var valid = !IsInvalid;
            input.Attack.Update(valid && (GetKeysDown(KeyCode.E) || _controlMapping.Attack(Player.Controller)));
            input.Special.Update(valid && (GetKeysDown(KeyCode.S) || _controlMapping.Special(Player.Controller)));
            input.Shield.Update(valid && (GetKeysDown(KeyCode.LeftShift) || _controlMapping.Shield(Player.Controller)));
            input.Jump.Update(Jump);
            context.Input = input;
        }

        void IDataComponent<Player>.SetData(Player data) {
            Player = data;
        }

        Vector2 DirectionClamp(Vector2 dir) {
            dir.x = Mathf.Clamp(dir.x, -1, 1); 
            dir.y = Mathf.Clamp(dir.y, -1, 1); 
            return dir;
        }

        bool GetKeys(params KeyCode[] keys) {
            return keys.Any(Input.GetKey);
        }

        bool GetKeysDown(params KeyCode[] keys) {
            return keys.Any(Input.GetKeyDown);
        }

        float ButtonAxis(bool neg, bool pos) {
            var val = neg ? -1f : 0f;
            return val + (pos ? 1f : 0f);
        }

        protected override void Awake() {
            base.Awake();
            //TODO(james7132): Generalize this 
            _controlMapping = new PlayerControlMapping();
        }

    }

    public class TapDetector {

        readonly float _deadZone;
        Vector2 _acceleration;
        Vector2 _value;
        Vector2 _velocity;

        public TapDetector(float deadZone) { _deadZone = deadZone; }

        int Sign(float x) {
            if (x > 0)
                return 1;
            if (x < 0)
                return -1;
            return 0;
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
            _value = input;
            return MaxComponent(Snap(input, _acceleration));
        }

    }

}
