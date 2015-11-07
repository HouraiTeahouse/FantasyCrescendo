using System;
using UnityEngine;
using System.Collections;

namespace Hourai {

    public class JoystickAxis : IInputAxis {

        private int _joyNum;
        private int _axisNum;
        private string _axisString;

        public int JoyNum {
            get { return _joyNum; }
            set {
                if (value < 1 || value > 20)
                    throw new ArgumentException("JoyNum must between 1 and 4");
                _joyNum = value;
                _axisString = _joyNum + "_" + _axisNum;
            }
        }

        public int AxisNum {
            get { return _axisNum; }
            set {
                if (value < 1 || value > 20)
                    throw new ArgumentException("AxisNum must between 1 and 20");
                _axisNum = value;
                _axisString = _joyNum + "_" + _axisNum;
            }
        }

        public JoystickAxis(int axisNum, int joyNum = 0) {
            AxisNum = axisNum;
            JoyNum = joyNum;
        }

        public void Update() {}

        public float GetValue() {
            return UnityEngine.Input.GetAxisRaw(_axisString);
        }

    }

}
