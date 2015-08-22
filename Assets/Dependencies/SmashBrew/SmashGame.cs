using System;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    public static class SmashExtensions {

        public static bool IsPlayer(this GameObject gameObj) {
            return gameObj != null && gameObj.CompareTag(SmashGame.Config.PlayerTag);
        }

        public static bool IsPlayer(this Component obj) {
            return obj != null && obj.CompareTag(SmashGame.Config.PlayerTag);
        }

        public static bool IsHurtbox(this Collider collider) {
            return collider != null && ((1 << collider.gameObject.layer) & SmashGame.Config.HurtboxLayers) != 0;
        }

    }

    public class SmashGame : ConfigurableGame<SmashConfig> {

        public static Transform[] GetSpawnPoints() {
            return GetPoints(Config.spawnTag);
        }

        private static Transform[] GetPoints(string tag) {
            return GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform).ToArray();
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
            switch (type) {
                case Hitbox.Type.Offensive:
                    return Config.OffensiveHitboxColor;
                case Hitbox.Type.Damageable:
                    return Config.DamageableHitboxColor;
                case Hitbox.Type.Invincible:
                    return Config.IntangibleHitboxColor;
                case Hitbox.Type.Intangible:
                    return Config.InvincibleHitboxColor;
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