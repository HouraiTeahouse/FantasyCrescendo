using UnityEngine;

namespace Crescendo.API {

    public class FallThrough : TriggerStageElement {

        private void Check(Collider col) {
            if (!col.CompareTag(Game.PlayerTag))
                return;

            var character = col.gameObject.GetComponentInParent<Character>();
            if (character == null)
                return;

            if (character.InputSource.Crouch)
                ChangeIgnore(col, true);
        }

        private void OnCollisionStay(Collision col) {
            Check(col.collider);
        }

        private void OnCollisionEnter(Collision col) {
            Check(col.collider);
        }

        private void OnTriggerExit(Collider other) {
            ChangeIgnore(other, false);
        }

    }

}