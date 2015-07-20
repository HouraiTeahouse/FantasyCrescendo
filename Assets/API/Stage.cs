using System;
using UnityEngine;

namespace Genso.API {

    /// <summary>
    /// A container behaviour for handling general data about the stage.
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public sealed class Stage : Singleton<Stage>
    {

        [SerializeField]
        private Camera mainCamera;

        private SpawnPoint[] spawnPoints;
        private Transform respawnPoint;

        public static Transform Transform {
            get { return Instance.transform; }
        }

        public static Vector3 RespawnPosition {
            get { return Instance.respawnPoint.position; }
        }

        /// <summary>
        /// The maximum number of supported players on this Stage
        /// </summary>
        public static int SupportedPlayerCount
        {
            get { return Instance.spawnPoints.Length; }
        }

        public static Camera Camera
        {
            get
            {
                return Instance.mainCamera;
            }
        }

		public static SpawnPoint GetSpawnPoint(int playerNumber) {
			if(Instance == null)
				throw new InvalidOperationException("Cannot get the spawn points of a stage that does not exist");
			if(playerNumber < 0 || playerNumber > SupportedPlayerCount)
				throw new InvalidOperationException();
			return Instance.spawnPoints[playerNumber];
		}

        protected override void Awake()
        {
            base.Awake();
            spawnPoints = FindObjectsOfType<SpawnPoint>();
            respawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

            // Sort the Spawn Points by name instead of by random spatial orientation
            Array.Sort(spawnPoints, (s1, s2) => s1.name.CompareTo(s2.name));

            if (mainCamera != null)
                return;

            mainCamera = Camera.main ?? FindObjectOfType<Camera>();

            if (mainCamera == null)
                Debug.LogError("Stage has no Camera!");
        }

    }


}
