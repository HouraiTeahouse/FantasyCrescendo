using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hourai.SmashBrew {

    /// <summary>
    /// A container behaviour for handling general data about the stage.
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public sealed class Stage : Singleton<Stage> {

        [SerializeField]
        private BGMGroup backgroundMusic;

        private Transform[] spawnPoints;

        public static Transform Transform {
            get { return Instance.transform; }
        }

        public static Vector3 Up {
            get { return Transform.up; }
        }

        public static Vector3 Right {
            get { return Transform.right; }
        }

        public static Vector3 Forward {
            get { return Transform.forward; }
        }

        /// <summary>
        /// The maximum number of supported players on this Stage
        /// </summary>
        public static int SupportedPlayerCount {
            get { return Instance.spawnPoints.Length; }
        }

        public static Transform GetSpawnPoint(int playerNumber) {
            if (Instance == null)
                throw new InvalidOperationException("Cannot get the spawn points of a stage that does not exist");
            if (playerNumber < 0 || playerNumber > SupportedPlayerCount)
                throw new InvalidOperationException();
            return Instance.spawnPoints[playerNumber];
        }

        protected override void Awake() {
            base.Awake();

            spawnPoints = SmashGame.GetSpawnPoints();

            // Sort the Spawn Points by name instead of by random spatial orientation
            Array.Sort(spawnPoints, (s1, s2) => s1.name.CompareTo(s2.name));
        }

    }

}