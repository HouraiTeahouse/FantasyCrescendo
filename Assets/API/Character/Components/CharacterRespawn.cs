using UnityEngine;

namespace Crescendo.API {


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
            Transform respawnPos = Stage.RespawnPosition;
            Character.position = respawnPos.position;
            Character.rotation = respawnPos.rotation;
            Game.CreateRespawnPlatform(Character);
        }

    }

}
