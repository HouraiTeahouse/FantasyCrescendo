using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {
    
    [DefineCategories("Tags", "Debug")]
    public class SmashConfig : GameConfig {

        private Resource<GameObject> _Respawn;

        [SerializeField, ResourcePath(typeof(GameObject))]
        private string _respawnPlatformPrefab;
        
        public PlayerData[] GenericPlayerData;
        public LayerMask HurtboxLayers;

        [Tags, Category("Tags")]
        public string spawnTag;

        public RespawnPlatform RepsawnPlatformPrefab {
            get { return _Respawn.Load().GetComponent<RespawnPlatform>(); }
        }

        private void OnEnable() {
            _Respawn = new Resource<GameObject>(_respawnPlatformPrefab);
        }

        public readonly Color CPUColor = Color.grey;

        [Category("Debug")]
        public readonly Color DamageableHitboxColor = Color.yellow;

        [Category("Debug")]
        public readonly Color IntangibleHitboxColor = Color.blue;

        [Category("Debug")]
        public readonly Color InvincibleHitboxColor = Color.green;

        [Category("Debug")]
        public readonly Color OffensiveHitboxColor = Color.red;

        [System.Serializable]
        public class PlayerData {

            public Color Color;
            public Sprite IndicatorSprite;

        }
        
    }

}