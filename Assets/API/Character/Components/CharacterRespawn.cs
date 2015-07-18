using UnityEngine;
using Genso.API;

namespace Genso.API {


    public class CharacterRespawn : CharacterComponent {

        [SerializeField]
        private RespawnPlatform RespawnPlatform;

        [SerializeField]
        private float _invincibilityTime = 2f;

        [SerializeField]
        private float platformTimer = 4f;

        public override void OnBlastZoneExit() {
            
            Vector3 respawnPos = Stage.RespawnPosition;
            Character.transform.position = respawnPos;
            if (RespawnPlatform == null)
                return;
            RespawnPlatform instance = RespawnPlatform.Copy(respawnPos);
            instance.Character = Character;
            instance.InvincibilityTimer = _invincibilityTime;
            instance.PlatformTimer = platformTimer;
        }

    }

}
