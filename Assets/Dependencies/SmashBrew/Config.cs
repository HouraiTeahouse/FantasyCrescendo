using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [CreateAssetMenu(fileName = "New Config", menuName = "SmashBrew/Config")]
    public sealed class Config : ScriptableObject {
        #region Serialized Fields
        [SerializeField]
        private GameModeConfig _gameModes;

        [SerializeField]
        private PhysicsConfig _physics;

        [SerializeField]
        private PlayerConfig _player;

        [SerializeField]
        private DebugConfig _debug;
        #endregion
        private static Config _instance;

        /// <summary>
        /// The singleton instance of the game's config
        /// </summary>
        static Config Instance {
            get {
                if (_instance)
                    return _instance;
                _instance = Resources.Load<Config>("Config");
                if(!_instance)
                    _instance = CreateInstance<Config>();
                return _instance; 
            }
        }

        public static PlayerConfig Player {
            get { return Instance._player; }
        }

        public static PhysicsConfig Physics {
            get { return Instance._physics; }
        }

        public static GameModeConfig GameModes {
            get { return Instance._gameModes; }
        }

        public static DebugConfig Debug {
            get { return Instance._debug; }
        }

        /// <summary>
        /// Unity callback. Called on load.
        /// </summary>
        void OnEnable() {
            //TODO: Generalize
            GameMode.Current = _gameModes.StandardVersus;
        }
    }

    [Serializable]
    public class DebugConfig {
        [SerializeField]
        private Color DamageableHitboxColor = Color.yellow;

        [SerializeField]
        private Color IntangibleHitboxColor = Color.blue;

        [SerializeField]
        private Color InvincibleHitboxColor = Color.green;

        [SerializeField]
        private Color OffensiveHitboxColor = Color.red;
        
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

    [Serializable]
    public class GameModeConfig {
        [SerializeField]
        private SerializedGameMode _standardVersus;

        [SerializeField]
        private SerializedGameMode _training;

        [SerializeField]
        private SerializedGameMode _arcade;

        [SerializeField]
        private SerializedGameMode _allStar;

        public GameMode StandardVersus {
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
    }

    [Serializable]
    public class PlayerConfig {
        [SerializeField]
        private Color[] _playerColors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            new Color(1, 0.5f, 0), Color.cyan, Color.magenta, new Color(0.25f, 0.25f, 0.25f)
        };

        [SerializeField]
        private Color _cpuColor = new Color(0.75f, 0.75f, 0.75f);

        [SerializeField]
        private float _tapPersistence = 1 / 12f;

        [SerializeField]
        private float _tapTreshold = 0.3f;

        public Color CPUColor {
            get { return _cpuColor; }
        }

        /// <summary>
        /// How long a tap persists before it is no longer valid
        /// </summary>
        public float TapPersistence {
            get { return _tapPersistence; }
        }

        /// <summary>
        /// Minimum acceleration (normalized controller units/second) for a tap to be considered a tap
        /// </summary>
        public float TapTreshold {
            get { return _tapTreshold; }
        }

        public Color GetColor(int playerNumber, bool isCPU = false) {
            if(isCPU)
                return _cpuColor;
            return _playerColors[playerNumber % _playerColors.Length];
        }
    }

    [Serializable]
    public class PhysicsConfig {
        [SerializeField]
        private float _tangibleSpeedCap = 3f;

        public float TangibleSpeedCap {
            get { return _tangibleSpeedCap; }
        }
    }
}
