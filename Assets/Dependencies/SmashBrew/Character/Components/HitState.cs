using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class HitState : CharacterNetworkComponent {

        [SyncVar]
        float _hitstun;

        /// <summary>
        /// The amount of hitstun time remaining
        /// </summary>
        public float Hitstun {
            get { return _hitstun; }
            set { _hitstun = value; }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (isServer)
                return;
            _hitstun = Mathf.Max(0f, _hitstun - Time.deltaTime);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            context.Hitstun = Hitstun;
        }

    }

}
