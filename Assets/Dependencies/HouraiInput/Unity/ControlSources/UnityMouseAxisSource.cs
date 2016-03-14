using System;
using UnityEngine;


namespace HouraiTeahouse.HouraiInput {
    public class UnityMouseAxisSource : InputSource {
        string mouseAxisQuery;


        public UnityMouseAxisSource(string axis) {
            this.mouseAxisQuery = "mouse " + axis;
        }


        public float GetValue(InputDevice inputDevice) {
            return Input.GetAxisRaw(mouseAxisQuery);
        }


        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }
    }
}
