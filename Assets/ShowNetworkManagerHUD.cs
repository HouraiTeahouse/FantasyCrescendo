using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo  {

    public class ShowNetworkManagerHUD : MonoBehaviour {

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            State(true);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            State(false);
        }

        void State(bool state) {
            foreach (var hud in FindObjectsOfType(typeof(NetworkManagerHUD)).OfType<NetworkManagerHUD>())
                hud.enabled = state;
        }

    }

}