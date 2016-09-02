using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A MonoBehaviour that restricts the posiiton of all of the children of the GameObject it is attached to. </summary>
    public class RestrainChildren : MonoBehaviour {

        /// <summary> In local coordiates, the bounds for where the children of the GameObject can move </summary>
        [SerializeField]
        Bounds _bounds;

        /// <summary> Unity callback. Called once per frame after all Update calls. </summary>
        void LateUpdate() {
            foreach (Transform child in transform)
                child.localPosition = _bounds.ClosestPoint(child.localPosition);
        }

        /// <summary> Unity callback. Called in the editor to draw gizmos in the scene view. </summary>
        void OnDrawGizmos() {
            using (Gizmo.With(Color.white, transform)) {
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }

    }

}