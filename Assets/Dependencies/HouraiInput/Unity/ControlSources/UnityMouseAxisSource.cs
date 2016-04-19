using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityMouseAxisSource : InputSource {
        readonly string _mouseAxisQuery;

        public UnityMouseAxisSource(string axis) {
            _mouseAxisQuery = "mouse " + axis;
        }

        public float GetValue(InputDevice inputDevice) {
            return Input.GetAxisRaw(_mouseAxisQuery);
        }

        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }
    }
}
