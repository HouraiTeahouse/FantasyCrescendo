using UnityEngine;
using System.Collections;

namespace Hourai {

    public class KeyboardAxis : IInputAxis {

        private KeyCode _positive;
        private KeyCode _negative;

        public KeyboardAxis(KeyCode positive, KeyCode negative) {
            _positive = positive;
            _negative = negative;
        }

        public float GetAxisValue() {
            float value = 0f;
            if (Input.GetKey(_positive))
                value += 1f;
            if (Input.GetKey(_negative))
                value -= 1f;
            return value;
        }
    }
}
