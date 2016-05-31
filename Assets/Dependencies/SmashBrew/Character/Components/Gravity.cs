using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public sealed class Gravity : MonoBehaviour {

        [SerializeField, Tooltip("The acceleration downward per second applied")]
        private float _gravity = 9.86f;

        /// <summary>
        /// Gets or sets the magnitude of gravity applied to the Character.
        /// </summary>
        public float Force {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        public static implicit operator float(Gravity gravity) {
            return gravity != null ? gravity.Force : 0f;
        }

    }
}
