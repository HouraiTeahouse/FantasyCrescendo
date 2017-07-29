using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(KnockbackState))]
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

        KnockbackState KnockbackState { get; set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            KnockbackState = this.SafeGetComponent<KnockbackState>();
            if (KnockbackState == null)
                return;
            KnockbackState.OnHit += (src, dir) => {
                var hitbox = src as Hitbox;
                if (hitbox == null)
                    return;
                Hitstun = Config.Fight.CalculateHitstun(hitbox.Damage);
            };
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (_hitstun <= 0f)
                return;
            _hitstun = Mathf.Max(0f, _hitstun - Time.deltaTime);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            context.Hitstun = Hitstun;
        }

        public override void ResetState() {
            Hitstun = 0f;
        }

    }

}
