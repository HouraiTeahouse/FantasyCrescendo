using System;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    public class Game : Singleton<Game> {

        #region Global Callbacks 

        public static event Action OnUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnFixedUpdate;
        public static event Action<int> OnLoad;
        public static event Action OnApplicationFocused;
        public static event Action OnApplicationUnfocused;
        public static event Action OnApplicationExit;

        private void Update() {
            OnUpdate.SafeInvoke();
        }

        private void LateUpdate() {
            OnLateUpdate.SafeInvoke();
        }

        private void FixedUpdate() {
            OnFixedUpdate.SafeInvoke();
        }

        private void OnApplicationFocus(bool focus) {
            if (focus)
                OnApplicationFocused.SafeInvoke();
            else
                OnApplicationUnfocused.SafeInvoke();
        }

        private void OnApplicationQuit() {
            OnApplicationExit.SafeInvoke();
        }

        private void OnLevelWasLoaded(int level) {
            OnLoad.SafeInvoke(level);
        }

        #endregion
        
        [Serialize, Inline]
        private Config _config;

        private static Config Config {
            get {
                return Instance == null ? null : Instance._config;
            }
        }

        public static bool IsPlayer(Component obj) {
            return obj.CompareTag(Config.playerTag);
        }

        public static Transform[] GetSpawnPoints() {
            return GetPoints(Config.spawnTag);
        }

        public static Transform[] GetRespawnPoint() {
            return GetPoints(Config.respawnTag);
        }

        private static Transform[] GetPoints(string tag) {
            return GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform).ToArray();
        }

        public static bool IsHurtbox(Component obj) {
            return IsHurtbox(obj.gameObject);
        }

        public static bool IsHurtbox(GameObject obj) {
            return ((1 << obj.layer) & Config.HurtboxLayers) != 0;
        }

        public static int MaxPlayers {
            get { return Config.GenericPlayerData.Length; }
        }

        protected override void Awake() {
            base.Awake();
            if (_config == null) {
                Config[] configs = Resources.FindObjectsOfTypeAll<Config>();
                if (configs.Length > 0)
                    _config = configs[0];
                else {
                    Debug.LogError(
                                   "Game singledton does not have an assigned Config and no configs are found in resources");
                }
            }
        }

        public static Character SpawnPlayer(int playerNumber, Character characterPrefab) {
            // Instantiate a instance of the Character
            Character instance = characterPrefab.InstantiateNew();

            // Set Player's number
            instance.PlayerNumber = playerNumber;

            // Create the player's indicator
            var newIndicator =
                new GameObject("P" + (playerNumber + 1) + " Indicator").AddComponent<PlayerIndicator>();
            newIndicator.Color = GetPlayerColor(playerNumber);
            newIndicator.Sprite = (playerNumber >= 0 && playerNumber <= MaxPlayers)
                                      ? Config.GenericPlayerData[playerNumber].IndicatorSprite
                                      : null;

            // Hide the indicator objects only if it is in the Editor
#if UNITY_EDITOR
            newIndicator.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif

            //Attach the indicator to the Character instance
            newIndicator.Attach(instance);

            return instance;
        }

        public static void CreateRespawnPlatform(Character target) {
            if (target == null)
                throw new ArgumentNullException("target");
            RespawnPlatform platform = Config.RepsawnPlatformPrefab.InstantiateNew(target.position);
            platform.Character = target;
        }

        public static Color GetHitboxColor(Hitbox.Type type) {
            Config.DebugData debugData = Config.Debug;
            switch (type) {
                case Hitbox.Type.Offensive:
                    return debugData.OffensiveHitboxColor;
                case Hitbox.Type.Damageable:
                    return debugData.DamageableHitboxColor;
                case Hitbox.Type.Invincible:
                    return debugData.IntangiblHitboxColor;
                case Hitbox.Type.Intangible:
                    return debugData.InvincibleHitboxColor;
                default:
                    return Color.white;
            }
        }

        public static Color GetPlayerColor(int playerNumber) {
            if (Config == null)
                return Color.white;
            return playerNumber < 0 || playerNumber >= MaxPlayers
                       ? Color.white
                       : Config.GenericPlayerData[playerNumber].Color;
        }

    }

}