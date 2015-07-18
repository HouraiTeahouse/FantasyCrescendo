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

        public void StartMatch(Match match)
        {
            if (match == null)
                throw new ArgumentException("match");
            if (match.PlayerCount > SupportedPlayerCount)
                throw new InvalidOperationException("Cannot start a match when there are more players participating than supported by the selected stage");

            for (var i = 0; i < match.PlayerCount; i++) {
                Character spawnedCharacter = match.SpawnCharacter(i, spawnPoints[i]);
                PlayerIndicator indicator = GameSettings.CreatePlayerIndicator(i);
                indicator.Attach(spawnedCharacter);
            }

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
