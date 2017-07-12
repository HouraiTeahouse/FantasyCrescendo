using HouraiTeahouse.SmashBrew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    public class DebugStage : MonoBehaviour { 

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            Hitbox.DrawHitboxes = true;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            Hitbox.DrawHitboxes = false;
        }

    }

}
