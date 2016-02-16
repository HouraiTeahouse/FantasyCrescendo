using UnityConstants;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class FallThrough : TriggerStageElement {

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

        void OnCollisionStay(Collision col) {
            Check(col.collider);
        }

        void OnCollisionEnter(Collision col) {
            Check(col.collider);
        }

        void OnTriggerExit(Collider other) {
            ChangeIgnore(other, false);
        }

    }

}
