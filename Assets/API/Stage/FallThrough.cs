using UnityEngine;

namespace Crescendo.API {

    public class FallThrough : TriggerStageElement {

        private void OnCollisionEnter(Collision col) {
            if (!col.collider.CompareTag(Game.PlayerTag))
                return;

            var character = col.gameObject.GetComponentInParent<Character>();
            if (character == null)
                return;

            if (character.InputSource.Crouch)
                ChangeIgnore(col.collider, true);
        }

        private void OnTriggerExit(Collider other) {
            ChangeIgnore(other, false);
        }

    }

}