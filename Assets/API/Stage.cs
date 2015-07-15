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
        private Transform[] respawnPoints;

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
                Character spawnedCharacter = spawnPoints[i].Spawn(match.GetCharacterPrefab(i));
                PlayerIndicator indicator = GameSettings.CreatePlayerIndicator(i);
                indicator.Attach(spawnedCharacter);
            }

        }

        protected override void Awake()
        {
            base.Awake();
            spawnPoints = FindObjectsOfType<SpawnPoint>();
            if (mainCamera != null)
                return;

            mainCamera = Camera.main ?? FindObjectOfType<Camera>();

            if (mainCamera == null)
                Debug.LogError("Stage has no Camera!");
        }

    }


}
