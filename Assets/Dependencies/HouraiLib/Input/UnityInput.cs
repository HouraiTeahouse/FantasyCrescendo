using UnityEngine;

namespace Hourai {

    public class UnityInput : IInputButton, IInputAxis {

        private readonly string _name;

        public string Name {
            get { return _name; }
        }

        public UnityInput(string name) {
            _name = name;
        }

        public bool GetButtonValue() {
            return Input.GetButtonDown(_name);
        }

        public float GetAxisValue() {
            return Input.GetAxisRaw(_name);
        }

    }

}