using UnityEngine;
using System.Collections;

namespace Hourai {

    public class StartMatch : MonoBehaviour {

        void Awake() {
            Match.Begin();
        }

    }

}