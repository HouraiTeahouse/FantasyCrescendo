using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage { 

    [AddComponentMenu("Smash Brew/Stage/Platform")]
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
        void Awake() { _toIgnore = GetComponentsInChildren<Collider>().Where(col => col != null && !col.isTrigger).ToArray(); }

        /// <summary> Changes the ignore state of </summary>
        /// <param name="target"> </param>
        /// <param name="state"> </param>
        void ChangeIgnore(Collider target, bool state) {
            if (target == null || !target.CompareTag(Config.Tags.PlayerTag))
                return;
            var movementState = target.GetComponentInParent<MovementState>();
            foreach (Collider col in _toIgnore)
                if (movementState != null)
                    movementState.IgnoreCollider(col, state);
                else
                    Physics.IgnoreCollision(col, target, state);
        }

        /// <summary> Check if the </summary>
        /// <param name="col"> </param>
        void Check(Collider col) {
            if (!col.CompareTag(Config.Tags.PlayerTag))
                return;

            var character = col.gameObject.GetComponentInParent<Character>();
            var inputState = col.gameObject.GetComponentInParent<InputState>();
            if (character == null)
                return;
            var smash = Vector2.zero;
            if (inputState != null)
                smash = inputState.Smash;
            //TODO(james7132): Edit this to use normal input
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
                smash.y < -DirectionalInput.DeadZone) {
                ChangeIgnore(col, true);
                character.StateController.SetState(character.States.Fall);
            }
        }

        /// <summary> Unity callback. Called when another collider enters an attached trigger collider. </summary>
        void OnTriggerEnter(Collider other) { ChangeIgnore(other, true); }

        /// <summary> Unity callback. Called when another collider exits an attached trigger collider. </summary>
        void OnTriggerExit(Collider other) { ChangeIgnore(other, false); }

        public void CharacterCollision(CharacterController controller) {
            if (Hardness <= HardnessSetting.Soft)
                Check(controller);
        }

    }

}
