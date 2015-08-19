using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public sealed class CharacterRespawn : CharacterComponent {

        protected override void Start() {
            base.Start();

            if (Character == null)
                return;

            // Subscribe to Character event
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        private void OnDestroy() {
            // Unsubscribe to Character event
            if (Character)
                Character.OnBlastZoneExit -= OnBlastZoneExit;
        }

        private void OnBlastZoneExit() {
            if (!enabled)
                return;
            Transform respawnPos = Stage.RespawnPosition;
            Character.position = respawnPos.position;
            Character.rotation = respawnPos.rotation;
            Game.CreateRespawnPlatform(Character);
        }

    }

}