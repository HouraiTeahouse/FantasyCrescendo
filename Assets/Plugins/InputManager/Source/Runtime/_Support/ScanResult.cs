using UnityEngine;

namespace TeamUtility.IO {

    public struct ScanResult {

        public int joystick;
        public int joystickAxis;
        public KeyCode key;
        public int mouseAxis;
        public ScanFlags scanFlags;
        public object userData;

    }

}