using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class Facing : MonoBehaviour {

        private enum FacingMode {
            Rotation,
            Scale
        }

        [SerializeField]
        private FacingMode _facingMode;

        private bool _facing;

        /// <summary>
        /// The direction the character is currently facing.
        /// If set to true, the character faces the right.
        /// If set to false, the character faces the left.
        /// 
        /// The method in which the character is flipped depends on what the Facing Mode parameter is set to.
        /// </summary>
        public bool Direction {
            get {
                if (_facingMode == FacingMode.Rotation)
                    return transform.eulerAngles.y > 179f;
                return transform.localScale.x > 0;
            }
            set {
                if (_facing == value)
                    return;

                _facing = value;
                if (_facingMode == FacingMode.Rotation)
                    transform.Rotate(0f, 180f, 0f);
                else
                    transform.localScale *= -1;
            }
        }
    }
}
