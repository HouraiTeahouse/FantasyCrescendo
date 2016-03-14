using System;
using UnityEngine;


namespace HouraiTeahouse.HouraiInput {
    public class UnityKeyCodeAxisSource : InputSource {
        KeyCode negativeKeyCode;
        KeyCode positiveKeyCode;


        public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            this.negativeKeyCode = negativeKeyCode;
            this.positiveKeyCode = positiveKeyCode;
        }


        public float GetValue(InputDevice inputDevice) {
            int axisValue = 0;

            if (Input.GetKey(negativeKeyCode)) {
                axisValue--;
            }

            if (Input.GetKey(positiveKeyCode)) {
                axisValue++;
            }

            return axisValue;
        }


        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }
    }
}
