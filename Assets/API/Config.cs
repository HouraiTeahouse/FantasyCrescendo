using UnityEngine;
using System.Collections;

namespace Genso.API {


    public class Config : ScriptableObject {

        [Tag]
        public string playerTag;

        [Tag]
        public string spawnTag;

        [Tag]
        public string respawnTag;

        public LayerMask HurtboxLayers;

        [SerializeField, ResourcePath(typeof (GameObject))]
        private string _respawnPlatformPrefab;

        private Resource<GameObject> _Respawn;

        public RespawnPlatform RepsawnPlatformPrefab {
            get {
                return _Respawn.Load().GetComponent<RespawnPlatform>();
            }    
        }
        
        [System.Serializable]
        public class PlayerData
        {

            public Color Color;
            public Sprite IndicatorSprite;

        }

        [System.Serializable]
        public class DebugData
        {

            public Color OffensiveHitboxColor = Color.red;
            public Color DamageableHitboxColor = Color.yellow;
            public Color InvincibleHitboxColor = Color.green;
            public Color IntangiblHitboxColor = Color.blue;

        }

        public PlayerData[] GenericPlayerData;
        public DebugData Debug;

        void OnEnable() {
            _Respawn = new Resource<GameObject>(_respawnPlatformPrefab);
        }

    }

}

