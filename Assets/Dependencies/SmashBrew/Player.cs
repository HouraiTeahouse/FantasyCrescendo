using System;
using UnityEngine;

namespace Hourai.SmashBrew {
    
    public sealed class Player {

        public enum PlayerType {

            None = 0,
            CPU = 1,
            HumanPlayer = 2

        };

        private readonly int _playerNumber;

        public int PlayerNumber => _playerNumber;

        public CharacterData Character;

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

        public static Player GetPlayer(GameObject go) {
            if (!go)
                return null;
            var controller = go.GetComponentInParent<PlayerController>();
            return !controller ? null : controller.PlayerData;
        }

        public static Player GetPlayer(Component comp) {
            return !comp ? null : GetPlayer(comp.gameObject);
        }

        public static bool IsPlayer(GameObject go) {
            return GetPlayer(go) != null;
        }

        public static bool IsPlayer(Component comp) {
            return GetPlayer(comp) != null;
        }

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
            _playerNumber = number;
            CpuLevel = 3;
        }

        internal Character Spawn(Transform transform = null) {
            if(transform == null)
                throw new ArgumentNullException(nameof(transform));
            return Spawn(transform.position, transform.rotation);
        }

        internal Character Spawn(Vector3 pos, Quaternion rot) {
            Character prefab = Character.LoadPrefab();
            if (prefab == null)
                return null;
            SpawnedCharacter = prefab.Duplicate(pos, rot);
            var controller = SpawnedCharacter.GetComponentInChildren<PlayerController>();
            if (controller)
                controller.PlayerData = this;
            return SpawnedCharacter;
        }

        public override bool Equals(object obj) {
            if (obj is Player)
                return this == ((Player) obj);
            return false;
        }

        public override int GetHashCode() {
            return _playerNumber;
        }

        public static bool operator ==(Player p1, Player p2) {
            bool nullCheck1 = ReferenceEquals(p1, null);
            bool nullCheck2 = ReferenceEquals(p2, null);
            return !(nullCheck1 ^ nullCheck2) && (nullCheck1 || p1.PlayerNumber == p2.PlayerNumber);
        }

        public static bool operator !=(Player p1, Player p2) {
            return !(p1 == p2);
        }

    }

}