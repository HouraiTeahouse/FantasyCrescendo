using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityKeyCodeSource : InputSource {

        readonly KeyCode _keyCode;

        public UnityKeyCodeSource(KeyCode keycode = KeyCode.None) { _keyCode = keycode; }

        public float GetValue(InputDevice inputDevice) { return GetState(inputDevice) ? 1.0f : 0.0f; }

        public bool GetState(InputDevice inputDevice) { return Input.GetKey(_keyCode); }

    }

}