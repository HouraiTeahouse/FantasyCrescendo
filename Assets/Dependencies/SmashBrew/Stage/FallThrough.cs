using UnityEngine;

namespace Hourai.SmashBrew {

    public class FallThrough : TriggerStageElement {

        private void Check(Collider col) {
            if (!Game.IsPlayer(col))
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