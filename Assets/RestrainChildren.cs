using UnityEngine;
using System.Collections;

namespace Genso.API {


    [RequireComponent(typeof(BoxCollider))]
    public class RestrainChildren : MonoBehaviour {

        private BoxCollider bounds;

        void Awake() {
            bounds = GetComponent<BoxCollider>();
        }


        void LateUpdate() {
            var boundedArea = new Bounds(bounds.center, bounds.size);
            foreach (Transform child in transform)
                child.localPosition = boundedArea.ClosestPoint(child.localPosition);
        }
    }


}