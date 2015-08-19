using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [DefineCategories("Tags")]
    public class Config : BaseScriptableObject {

        private Resource<GameObject> _Respawn;

        [SerializeField, ResourcePath]
        private string _respawnPlatformPrefab;

        public DebugData Debug;
        public PlayerData[] GenericPlayerData;
        public LayerMask HurtboxLayers;

        [Tags, Category("Tags")]
        public string playerTag;

        [Tags, Category("Tags")]
        public string respawnTag;

        [Tags, Category("Tags")]
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