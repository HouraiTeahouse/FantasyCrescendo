using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    // This object, when enabled, will make the object persist through scene changes
    public class PersistentObject : MonoBehaviour {

        void Start() {
            DontDestroyOnLoad(this);
        }

    }

}