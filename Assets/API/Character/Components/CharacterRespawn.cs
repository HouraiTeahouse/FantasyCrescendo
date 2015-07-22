using UnityEngine;

namespace Genso.API {


    public sealed class CharacterRespawn : CharacterComponent {

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
            if (!enabled)
                return;
            Vector3 respawnPos = Stage.RespawnPosition;
            Character.transform.position = respawnPos;
            Game.CreateRespawnPlatform(Character);
        }

    }

}
