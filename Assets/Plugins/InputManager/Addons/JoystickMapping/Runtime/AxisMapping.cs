using UnityEngine;

namespace TeamUtility.IO {

    [System.Serializable]
    public class AxisMapping {

        private int _joystickAxis;
        private KeyCode _key;
        private string _name;
        private MappingWizard.ScanType _scanType;

        public AxisMapping(string name, KeyCode key) {
            _name = name;
            _key = key;
            _joystickAxis = -1;
            _scanType = MappingWizard.ScanType.Button;
        }

        public AxisMapping(string name, int joystickAxis) {
            _name = name;
            _joystickAxis = joystickAxis;
            _key = KeyCode.None;
            _scanType = MappingWizard.ScanType.Axis;
        }

        public string Name {
            get { return _name; }
        }

        public KeyCode Key {
            get { return _key; }
        }

        public int JoystickAxis {
            get { return _joystickAxis; }
        }

        public MappingWizard.ScanType ScanType {
            get { return _scanType; }
        }

    }

}