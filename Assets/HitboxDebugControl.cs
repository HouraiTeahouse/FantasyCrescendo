using UnityEngine;
using InControl;

namespace HouraiTeahouse.SmashBrew {

    public class HitboxDebugControl : MonoBehaviour {

        [SerializeField]
        private InputControlTarget _button = InputControlTarget.Select;

        void Update() {
            foreach (var devices in InputManager.Devices) {
                Debug.Log(devices.GetControl(_button).WasPressed);
                if (devices.GetControl(_button).WasPressed)
                    Hitbox.DrawHitboxes = !Hitbox.DrawHitboxes;
            }
        }

    }
}
