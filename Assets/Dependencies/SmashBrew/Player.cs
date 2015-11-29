using System;
using UnityEngine;

namespace Hourai.SmashBrew {
    
    public sealed class Player {

        public enum PlayerType {

            None = 0,
            CPU = 1,
            HumanPlayer = 2

        };

        public int PlayerNumber { get; private set; }

        public CharacterData Character;

        private Character _spawnedInstance;

        public int CpuLevel { get; set; }

        public int Pallete { get; set; }

        public PlayerType Type { get; set; }

        public IInputController Controller {
            get {
                InputManager manager = InputManager.Instance;
                if (!manager)
                    return null;
                return manager.GetController(PlayerNumber);
            }
        }

        public Character SpawnedCharacter { get; private set; }

        public static PlayerType GetNextType(PlayerType pt) {
            return pt + 1 > PlayerType.HumanPlayer ? PlayerType.None : pt + 1;
        }

        public void CycleType() {
            Type = GetNextType(Type);
            if (Type == PlayerType.None)
                Character = null;
        }

        public Color Color {
            get {
                if (Type == PlayerType.None)
                    return Color.clear;
                Config config = Config.Instance;
                if (Type == PlayerType.CPU)
                    return config.CPUColor;
                return config.GetPlayerColor(PlayerNumber);
            }
        }

        internal Player(int number) {
            PlayerNumber = number;
            CpuLevel = 3;
        }

        internal Character Spawn(Transform transform = null) {
            if(transform == null)
                throw new ArgumentNullException("transform");
            return Spawn(transform.position, transform.rotation);
        }

        internal Character Spawn(Vector3 pos, Quaternion rot) {
            Character prefab = Character.LoadPrefab(Pallete);
            if (prefab == null)
                return null;
            SpawnedCharacter = prefab.Duplicate(pos, rot);
            var controller = SpawnedCharacter.GetComponentInChildren<PlayerController>();
            if (controller)
                controller.PlayerData = this;
            return SpawnedCharacter;
        }

    }

}