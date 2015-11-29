using System;
using UnityEngine;

namespace Hourai {

    public class JoystickButton : IInputButton {

        private KeyCode _keyCode;

        public JoystickButton(int joyNum, int buttonNum) {
            if(joyNum < 0 || joyNum > 11 || buttonNum < 0 || buttonNum > 19)
                throw new ArgumentException();
            _keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), "Joystick" + ((joyNum == 0) ? string.Empty : joyNum.ToString()) + "Button" + buttonNum);
        }

        public JoystickButton(KeyCode keyCode) {
            string asString = keyCode.ToString();
            if(!asString.Contains("Joystick") || !asString.Contains("Button"))
                throw new ArgumentException();
            _keyCode = keyCode;
        }

        public bool GetButtonValue() {
            return Input.GetKey(_keyCode);
        }

    }

}
