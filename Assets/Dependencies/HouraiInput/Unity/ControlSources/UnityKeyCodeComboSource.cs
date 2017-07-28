using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityKeyCodeComboSource : InputSource {

        public UnityKeyCodeComboSource(params KeyCode[] keyCodeList) { KeyCodeList = keyCodeList; }

        public KeyCode[] KeyCodeList { get; set; }

        public float GetValue(InputDevice inputDevice) { return GetState(inputDevice) ? 1.0f : 0.0f; }

        public bool GetState(InputDevice inputDevice) { return KeyCodeList.Any(Input.GetKey); }

    }

}