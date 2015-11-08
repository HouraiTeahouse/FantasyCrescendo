using System;
using System.Linq;
using UnityConstants;
using UnityEngine;

namespace Hourai.SmashBrew {

    public static class SmashExtensions {

        public static bool IsPlayer(this GameObject gameObj) {
            return gameObj && gameObj.CompareTag(Tags.Player);
        }

        public static bool IsPlayer(this Component obj) {
            return obj && obj.CompareTag(Tags.Player);
        }

        public static bool IsHurtbox(this Collider collider) {
            return collider && collider.gameObject.layer == Layers.Hurtbox;
        }

    }

    public class SmashGame : Game {

        [SerializeField]
        private GameConfig _config;

        public static GameConfig Config {
            get { return Instance ? (Instance as SmashGame)._config : null;  }
        }

        public static Transform[] GetSpawnPoints() {
            return GetPoints(Tags.Spawn);
        }

        private static Transform[] GetPoints(string tag) {
            return GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform).ToArray();
        }

        public static int MaxPlayers {
            get { return Config.PlayerColors.Length; }
        }

        public static void CreateRespawnPlatform(Character target) {
            if (target == null)
                throw new ArgumentNullException("target");
            RespawnPlatform platform = Instantiate(Config.RepsawnPlatformPrefab);
            platform.transform.position = target.transform.position;
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
                       : Config.PlayerColors[playerNumber];
        }

    }

}