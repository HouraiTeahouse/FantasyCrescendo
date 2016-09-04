using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class MenuWiggle : MonoBehaviour {

        [SerializeField]
        InputTarget _horizontalAxis = InputTarget.RightStickX;

        [SerializeField]
        Vector2 _scale = new Vector2(30, 30);

        [SerializeField]
        InputTarget _verticalAxis = InputTarget.RightStickY;

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            Vector2 distortion = Vector2.zero;
            foreach (InputDevice device in HInput.Devices.IgnoreNulls()) {
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
