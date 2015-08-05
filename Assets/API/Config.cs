using UnityEngine;

namespace Crescendo.API {

    public class Config : ScriptableObject {

        private Resource<GameObject> _Respawn;

        [SerializeField, ResourcePath(typeof (GameObject))]
        private string _respawnPlatformPrefab;

        public DebugData Debug;
        public PlayerData[] GenericPlayerData;
        public LayerMask HurtboxLayers;

        [Tag]
        public string playerTag;

        [Tag]
        public string respawnTag;

        [Tag]
        public string spawnTag;

        public RespawnPlatform RepsawnPlatformPrefab {
            get { return _Respawn.Load().GetComponent<RespawnPlatform>(); }
        }

        private void OnEnable() {
            _Respawn = new Resource<GameObject>(_respawnPlatformPrefab);
        }

        [System.Serializable]
        public class PlayerData {

            public Color Color;
            public Sprite IndicatorSprite;

        }

        [System.Serializable]
        public class DebugData {

            public Color DamageableHitboxColor = Color.yellow;
            public Color IntangiblHitboxColor = Color.blue;
            public Color InvincibleHitboxColor = Color.green;
            public Color OffensiveHitboxColor = Color.red;

        }

    }

}