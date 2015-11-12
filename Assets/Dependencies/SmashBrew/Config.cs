using UnityEngine;

namespace Hourai.SmashBrew {
    
    public class Config : ScriptableObject {

        #region Serialized Fields

        [SerializeField]
        private Color[] PlayerColors = { Color.red, Color.blue, Color.green, Color.yellow };

        [SerializeField]
        private Color CPUColor = Color.grey;

        [SerializeField]
        private Color DamageableHitboxColor = Color.yellow;

        [SerializeField]
        private Color IntangibleHitboxColor = Color.blue;

        [SerializeField]
        private Color InvincibleHitboxColor = Color.green;
        
        [SerializeField]
        private Color OffensiveHitboxColor = Color.red;

        #endregion

        private static Config _instance;

        public static Config Instance {
            get {
                if(_instance)
                    return _instance;
                Config[] configs = Resources.LoadAll<Config>(string.Empty);
                if (configs.Length > 0)
                    return _instance = configs[0];
                return _instance = CreateInstance<Config>();
            }
        }

        public int MaxPlayers {
            get { return Instance.PlayerColors.Length; }
        }
        
        public Color GetPlayerColor(int playerNumber, bool CPU = false) {
            if (CPU)
                return Instance.CPUColor;
            return playerNumber < 0 || playerNumber >= MaxPlayers
                       ? Color.white
                       : Instance.PlayerColors[playerNumber];
        }

        public Color GetHitboxColor(Hitbox.Type type) {
            switch (type) {
                case Hitbox.Type.Offensive:
                    return Instance.OffensiveHitboxColor;
                case Hitbox.Type.Damageable:
                    return Instance.DamageableHitboxColor;
                case Hitbox.Type.Invincible:
                    return Instance.IntangibleHitboxColor;
                case Hitbox.Type.Intangible:
                    return Instance.InvincibleHitboxColor;
                default:
                    return Color.magenta;
            }
        }
        
    }

}