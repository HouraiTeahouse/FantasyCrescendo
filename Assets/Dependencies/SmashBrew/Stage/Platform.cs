using System.Linq;
using UnityConstants;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public sealed class Platform : MonoBehaviour {

        public enum HardnessSetting {

            // Both ways + can be knocked through 
            Supersoft = 0,
            // Both ways
            Soft = 1,
            // Only can be jumped through from the bottom
            // Cannot be fallen through
            Semisoft = 2

        }

        [SerializeField]
        [Tooltip("The hardness of the platform")]
        HardnessSetting _hardness = HardnessSetting.Soft;

        Collider[] _toIgnore;

        public HardnessSetting Hardness {
            get { return _hardness; }
            set { _hardness = value; }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() { _toIgnore = GetComponentsInChildren<Collider>().Where(col => col && col.isTrigger).ToArray(); }

        /// <summary> Changes the ignore state of </summary>
        /// <param name="target"> </param>
        /// <param name="state"> </param>
        void ChangeIgnore(Collider target, bool state) {
            if (target == null || !target.CompareTag(Tags.Player))
                return;

            foreach (Collider col in _toIgnore)
                Physics.IgnoreCollision(col, target, state);
        }

        /// <summary> Check if the </summary>
        /// <param name="col"> </param>
        static void Check(Component col) {
            if (!col.CompareTag(Tags.Player))
                return;

            // TODO: Reimplement

            //var character = col.gameObject.GetComponentInParent<Character>();
            //if (character == null || character.InputSource == null)
            //    return;

            //if (character.InputSource.Crouch)
            //    ChangeIgnore(col, true);
        }

        /// <summary> Unity callback. Called when another collider enters an attached trigger collider. </summary>
        void OnTriggerEnter(Collider other) { ChangeIgnore(other, true); }

        /// <summary> Unity callback. Called when another collider exits an attached trigger collider. </summary>
        void OnTriggerExit(Collider other) { ChangeIgnore(other, false); }

        /// <summary> Unity callback. Called every physics loop for each for each . </summary>
        void OnCollisionStay(Collision col) {
            if (Hardness <= HardnessSetting.Soft)
                Check(col.collider);
        }

        /// <summary> Unity callback. Called when another collider enters an attached trigger collider. </summary>
        void OnCollisionEnter(Collision col) {
            if (Hardness <= HardnessSetting.Soft)
                Check(col.collider);
        }

    }

}