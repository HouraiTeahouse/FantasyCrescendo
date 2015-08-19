using System;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    public class SmashGame : ConfigurableGame<SmashConfig> {

        public static bool IsPlayer(Component obj) {
            return obj.CompareTag(Config.PlayerTag);
        }

        public static Transform[] GetSpawnPoints() {
            return GetPoints(Config.spawnTag);
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
            SmashConfig.DebugData debugData = Config.Debug;
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