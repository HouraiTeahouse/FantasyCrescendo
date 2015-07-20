using UnityEngine;

namespace Genso.API {


    public sealed class CharacterRespawn : CharacterComponent {

        [SerializeField]
        private RespawnPlatform RespawnPlatform;

        protected override void Awake() {
            base.Awake();

            if (Character == null)
                return;

            // Subscribe to Character event
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        void OnDestroy() {
            // Unsubscribe to Character event
            if (Character)
                Character.OnBlastZoneExit -= OnBlastZoneExit;
        }

        void OnBlastZoneExit() {
            Vector3 respawnPos = Stage.RespawnPosition;
            Character.transform.position = respawnPos;
            if (RespawnPlatform == null)
                return;
            RespawnPlatform.Copy(respawnPos);
        }

    }

}
