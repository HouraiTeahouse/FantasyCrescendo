using UnityEngine;

namespace Hourai.SmashBrew {
    
    public class SmashConfig : GameConfig {

        private Resource<GameObject> _Respawn;

        [SerializeField]
        private string _respawnPlatformPrefab;
        
        public Color[] PlayerColors;
        public LayerMask HurtboxLayers;
        public string spawnTag;

        public RespawnPlatform RepsawnPlatformPrefab {
            get { return _Respawn.Load().GetComponent<RespawnPlatform>(); }
        }

        //protected override void OnEnable() {
        //    base.OnEnable();
        void OnEnable() {
            _Respawn = new Resource<GameObject>(_respawnPlatformPrefab);
        }

        public Color CPUColor = Color.grey;
  
        public Color DamageableHitboxColor = Color.yellow;
        public Color IntangibleHitboxColor = Color.blue;
        public Color InvincibleHitboxColor = Color.green;
        public Color OffensiveHitboxColor = Color.red;
        
    }

}