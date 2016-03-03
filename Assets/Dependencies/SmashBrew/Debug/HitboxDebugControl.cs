using UnityEngine;
using InControl;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// Debug mode to turn on and off the rendering of hitboxes
    /// </summary>
    public class HitboxDebugControl : MonoBehaviour {

        [SerializeField]
        private InputControlTarget _button = InputControlTarget.Select;

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        void Update() {
            foreach (var devices in InputManager.Devices) {
                if (devices.GetControl(_button).WasPressed)
                    Hitbox.DrawHitboxes = !Hitbox.DrawHitboxes;
            }
        }

    }
}
