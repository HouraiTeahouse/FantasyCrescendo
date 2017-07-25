using System;
using HouraiTeahouse.AssetBundles;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [CreateAssetMenu(fileName = "New Config", menuName = "SmashBrew/Config")]
    public sealed class Config : ExtendableObject {

        static Config _instance;

        /// <summary> The singleton instance of the game's config </summary>
        public static Config Instance {
            get {
                if (_instance)
                    return _instance;
                _instance = Resources.Load<Config>("Config");
                if (!_instance)
                    _instance = CreateInstance<Config>();
                return _instance;
            }
        }

        public static Config Load() { return Instance; }

        public static FightConfig Fight {
            get { return Instance._fight; }
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

        public static TagConfig Tags {
            get { return Instance._tags; }
        }

        /// <summary> Unity callback. Called on load. </summary>
        void OnEnable() {
            //TODO: Generalize
            _gameModes.RegisterAll();
            GameMode.Current = _gameModes.StandardVersus;
        }

        #region Serialized Fields

        [SerializeField]
        FightConfig _fight;

        [SerializeField]
        BundleConfig _bundles;

        [SerializeField]
        GameModeConfig _gameModes;

        [SerializeField]
        PhysicsConfig _physics;

        [SerializeField]
        PlayerConfig _player;

        [SerializeField]
        DebugConfig _debug;

        [SerializeField]
        TagConfig _tags;

        #endregion
    }

    [Serializable]
    public class FightConfig {

        [SerializeField]
        float _baseHitstun = 1/20f;

        [SerializeField]
        float _hitstunScaling = 1/60f;

        [SerializeField]
        float _maxHitstun = 1/3f;

        public float CalculateHitstun(float damage) {
            return Mathf.Min(_baseHitstun + (damage * _hitstunScaling), _maxHitstun);
        }

    }

    [Serializable]
    public class BundleConfig {

        [SerializeField, Multiline]
        string _url;

        [SerializeField]
        string _branch;

        const string BranchIdentifier = "%branch%";
        const string PlatformIdentifier = "%platform%";
        string _baseUrl = null;

        public string BaseUrl {
            get {
                if (!string.IsNullOrEmpty(_baseUrl))
                    return _baseUrl;
                _baseUrl = _url.Replace(BranchIdentifier, Branch)
                    .Replace(PlatformIdentifier, BundleUtility.GetPlatformName());
                return _baseUrl;
            }
        }

        public string Branch {
            get { return _branch; }
        }

    }

    [Serializable]
    public class TagConfig {

        [SerializeField, Tag]
        [Tooltip("The tag for marking player GameObjects")]
        string _playerTag;

        [SerializeField, Tag]
        [Tooltip("The tag for marking hitboxes")]
        string _hitboxTag;

        [SerializeField, Tag]
        [Tooltip("The tag for marking ledges")]
        string _ledgeTag;

        [SerializeField, Tag]
        [Tooltip("The tag for mark follow targets for Player Indicators")]
        string _indicatorTargetTag;

        [SerializeField, Layer]
        int _characterLayer;

        [SerializeField, Layer]
        int _intangibleLayer;

        [SerializeField, Layer]
        int _hitboxLayer;

        [SerializeField, Layer]
        int _hurtboxLayer;

        public string PlayerTag { get { return _playerTag; } }
        public string HitboxTag { get { return _hitboxTag; } } 
        public string LedgeTag { get { return _ledgeTag; } }
        public string IndicatorTargetTag { get { return _indicatorTargetTag; } }

        public int CharacterLayer { get { return _characterLayer; } } 
        public int IntangibleLayer { get { return _intangibleLayer; } }
        public int HitboxLayer { get { return _hitboxLayer; } }
        public int HurtboxLayer { get { return _hurtboxLayer; } }

    }

    [Serializable]
    public class DebugConfig : ISerializationCallbackReceiver {

        [SerializeField]
        Color _inactiveHitboxColor = Color.black;

        [SerializeField]
        Color _offensiveHitboxColor = Color.red;

        [SerializeField]
        Color _damageableHitboxColor = Color.yellow;

        [SerializeField]
        Color _intangibleHitboxColor = Color.blue;

        [SerializeField]
        Color _invincibleHitboxColor = Color.green;

        [SerializeField]
        Color _shieldHitboxColor = Color.magenta;

        [SerializeField]
        Color _absorbHitboxColor = Color.cyan;

        [SerializeField]
        Color ReflectHitboxColor = new Color(0, 0.25f, 0.5f, 1);

        EnumMap<Hitbox.Type, Color> _colorMap;

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() {
            _colorMap = new EnumMap<Hitbox.Type, Color>();
            _colorMap[Hitbox.Type.Inactive] = _inactiveHitboxColor;
            _colorMap[Hitbox.Type.Offensive] = _offensiveHitboxColor;
            _colorMap[Hitbox.Type.Damageable] = _damageableHitboxColor;
            _colorMap[Hitbox.Type.Invincible] = _invincibleHitboxColor;
            _colorMap[Hitbox.Type.Intangible] = _intangibleHitboxColor;
            _colorMap[Hitbox.Type.Absorb] = _absorbHitboxColor;
            _colorMap[Hitbox.Type.Shield] = _shieldHitboxColor;
            _colorMap[Hitbox.Type.Reflective] = ReflectHitboxColor;
        }

        public Color GetHitboxColor(Hitbox.Type type) { return _colorMap[type]; }

    }

    [Serializable]
    public class GameModeConfig {

        [SerializeField]
        SerializedGameMode _allStar;

        [SerializeField]
        SerializedGameMode _arcade;

        [SerializeField]
        SerializedGameMode _standardVersus;

        [SerializeField]
        SerializedGameMode _training;

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

        public void RegisterAll() {
            GameMode.Register(_allStar);
            GameMode.Register(_arcade);
            GameMode.Register(_standardVersus);
            GameMode.Register(_training);
        }

    }

    [Serializable]
    public class PlayerConfig {

        [SerializeField]
        Color _cpuColor = new Color(0.75f, 0.75f, 0.75f);

        [SerializeField]
        Color[] _playerColors = {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            new Color(1, 0.5f, 0),
            Color.cyan,
            Color.magenta,
            new Color(0.25f, 0.25f, 0.25f)
        };

        [SerializeField]
        float _tapPersistence = 1 / 12f;

        [SerializeField]
        float _tapTreshold = 0.3f;

        public Color CPUColor {
            get { return _cpuColor; }
        }

        /// <summary> How long a tap persists before it is no longer valid </summary>
        public float TapPersistence {
            get { return _tapPersistence; }
        }

        /// <summary> Minimum acceleration (normalized controller units/second) for a tap to be considered a tap </summary>
        public float TapTreshold {
            get { return _tapTreshold; }
        }

        public Color GetColor(int playerNumber, bool isCPU = false) {
            if (isCPU)
                return _cpuColor;
            return _playerColors[playerNumber % _playerColors.Length];
        }

    }

    [Serializable]
    public class PhysicsConfig {

        [SerializeField]
        float _tangibleSpeedCap = 3f;

        public float TangibleSpeedCap {
            get { return _tangibleSpeedCap; }
        }

    }

}
