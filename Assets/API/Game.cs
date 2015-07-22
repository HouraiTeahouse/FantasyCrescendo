using System;
using UnityEngine;

namespace Genso.API {


    public class Game : Singleton<Game> {

        [SerializeField]
        private Config _config;

        private static Config Config {
            get {
                if (Instance == null)
                    return null;
                return Instance._config;
            }
        }

		public static Character SpawnPlayer(int playerNumber, CharacterData charData) {
			// Load the prefab for the player
			Character character = charData.LoadPrefab((int)(UnityEngine.Random.value * charData.AlternativeCount));

			// Instantiate a instance of the Character
			Character instance = character.Copy();

			// Create the player's indicator
			PlayerIndicator newIndicator = new GameObject("P" + (playerNumber + 1) + " Indicator").AddComponent<PlayerIndicator>();
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
            RespawnPlatform platform = Config.RepsawnPlatformPrefab.Copy(target.position);
            platform.Character = target;
        }

        public static int MaxPlayers
        {
            get { return Config.GenericPlayerData.Length; }
        }

        public static LayerMask HurtboxLayers
        {
            get { return Config.HurtboxLayers; }
        }

        public static Color GetHitboxColor(HitboxType type)
        {
            Config.DebugData debugData = Config.Debug;
            switch (type)
            {
                case HitboxType.Offensive:
                    return debugData.OffensiveHitboxColor;
                case HitboxType.Damageable:
                    return debugData.DamageableHitboxColor;
                case HitboxType.Invincible:
                    return debugData.IntangiblHitboxColor;
                case HitboxType.Intangible:
                    return debugData.InvincibleHitboxColor;
                default:
                    return Color.white;
            }
        }

        public static Color GetPlayerColor(int playerNumber)
        {
            if (Config == null)
                return Color.white;
            return playerNumber < 0 || playerNumber >= MaxPlayers ? Color.white : Config.GenericPlayerData[playerNumber].Color;
        }

        void OnLevelWasLoaded(int level)
        {
            GameObject[] staticManagers = GameObject.FindGameObjectsWithTag("Static Managers");
            foreach (GameObject tagged in staticManagers)
            {
                if (tagged != gameObject)
                    Destroy(tagged);
            }
        }

    }

}
