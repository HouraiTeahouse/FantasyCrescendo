using UnityEngine;
using System.Collections;

namespace Crescendo.API {

    public class StartMatch : MonoBehaviour {

        void Awake() {
            Match.Begin();
        }

    }

}