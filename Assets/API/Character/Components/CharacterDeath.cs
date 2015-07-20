using UnityEngine;

namespace Genso.API {


    public class CharacterDeath : CharacterComponent {


        [SerializeField]
        private ParticleSystem deathPrefab;

        protected override void Awake() {
            base.Awake();
            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        void OnDestroy() {
            if(Character == null)
                return;

            // Unsubscribe to Character events
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        void OnBlastZoneExit() {

            Vector3 position = Character.transform.position;

            if (deathPrefab != null) {
                ParticleSystem copy = deathPrefab.Copy(position);
                copy.transform.LookAt(transform.position - position);
                copy.startColor = Character.PlayerColor;
            }

        }

    }

}
