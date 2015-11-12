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

        public static Transform[] GetSpawnPoints() {
            return GetPoints(Tags.Spawn);
        }

        private static Transform[] GetPoints(string tag) {
            return GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform).ToArray();
        }

    }

}