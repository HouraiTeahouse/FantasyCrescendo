using UnityEngine;
using HouraiTeahouse.HouraiInput;

namespace HouraiTeahouse.SmashBrew.UI {
    public class MenuWiggle : MonoBehaviour {
        [SerializeField] private InputTarget _horizontalAxis = InputTarget.RightStickX;

        [SerializeField] private InputTarget _verticalAxis = InputTarget.RightStickY;

        [SerializeField] private Vector2 _scale = new Vector2(30, 30);

        /// <summary>
        /// Unity Callback. Called once every frame.
        /// </summary>
        void Update() {
            Vector2 distortion = Vector2.zero;
            int count = HInput.Devices.Count;
            for (var i = 0; i < count; i++) {
                InputDevice device = HInput.Devices[i];
                if (device == null)
                    continue;
                float x = device[_verticalAxis];
                float y = device[_horizontalAxis];
                if (Mathf.Abs(distortion.x) < Mathf.Abs(x))
                    distortion.x = x;
                if (Mathf.Abs(distortion.y) < Mathf.Abs(y))
                    distortion.y = y;
            }
            distortion = Vector2.ClampMagnitude(distortion, 1f);
            distortion.x *= _scale.x;
            distortion.y *= _scale.y;
            transform.rotation = Quaternion.Euler(distortion);
        }
    }
}
