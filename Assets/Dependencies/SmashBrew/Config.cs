using System.Collections.ObjectModel;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [CreateAssetMenu(fileName = "New Config", menuName = "SmashBrew/Config")]
    public sealed class Config : ScriptableObject {

        #region Serialized Fields
        [Header("Players")]
        [SerializeField]
        private Color[] _playerColors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            new Color(1, 0.5f, 0), Color.cyan, Color.magenta, new Color(0.25f, 0.25f, 0.25f)
        };

        [SerializeField]
        private Color _cpuColor = new Color(0.75f, 0.75f, 0.75f);

        [Header("Debug")]
        [SerializeField]
        private Color DamageableHitboxColor = Color.yellow;

        [SerializeField]
        private Color IntangibleHitboxColor = Color.blue;

        [SerializeField]
        private Color InvincibleHitboxColor = Color.green;
        
        [SerializeField]
        private Color OffensiveHitboxColor = Color.red;

        [Header("Game Modes")]
        [SerializeField]
        private SerializedGameMode _standardVersus;

        [SerializeField]
        private SerializedGameMode _training;

        [SerializeField]
        private SerializedGameMode _arcade;

        [SerializeField]
        private SerializedGameMode _allStar; 
        #endregion

        private ReadOnlyCollection<Color> _colors;
        public ReadOnlyCollection<Color> PlayerColors {
            get { return _colors; } 
        }

        public Color CPUColor {
            get { return _cpuColor; }
        }

        public GameMode StandardVerus {
            get { return _standardVersus; }            
        }

        public GameMode Training {
            get { return _training; }
        }

        public GameMode Arcade {
            get { return _arcade; }
        }

        public GameMode AllStar {
            get { return _allStar; }
        }

        private static Config _instance;

        /// <summary>
        /// The singleton instance of the game's config
        /// </summary>
        public static Config Instance {
            get {
                if(_instance)
                    return _instance;
                var configs = Resources.LoadAll<Config>(string.Empty);
                if (configs.Length > 0)
                    return _instance = configs[0];
                return _instance = CreateInstance<Config>();
            }
        }

        /// <summary>
        /// Unity callback. Called on load.
        /// </summary>
        void OnEnable() {
            _colors = new ReadOnlyCollection<Color>(_playerColors);
            GameMode.Current = StandardVerus;
        }

        public Color GetHitboxColor(Hitbox.Type type) {
            switch (type) {
                case Hitbox.Type.Offensive:
                    return OffensiveHitboxColor;
                case Hitbox.Type.Damageable:
                    return DamageableHitboxColor;
                case Hitbox.Type.Invincible:
                    return IntangibleHitboxColor;
                case Hitbox.Type.Intangible:
                    return InvincibleHitboxColor;
                default:
                    return Color.magenta;
            }
        }
    }
}
