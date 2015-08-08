using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Crescendo.API {

    [DisallowMultipleComponent]
    public class CharacterDeath : CharacterComponent {

        [SerializeField]
        private ParticleSystem deathPrefab;

        protected override void Start() {
            base.Start();
            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        private void OnDestroy() {
            if (Character == null)
                return;

            // Unsubscribe to Character events
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        private void OnBlastZoneExit() {
            Vector3 position = Character.transform.position;

            if (deathPrefab != null) {
                ParticleSystem copy = deathPrefab.Copy(position);
                copy.transform.LookAt(transform.position - position);
                copy.startColor = Character.PlayerColor;
            }
        }

    }

}