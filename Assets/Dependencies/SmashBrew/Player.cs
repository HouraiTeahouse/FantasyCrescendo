using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

namespace Hourai.SmashBrew {
    
    public sealed class Player {

        private static Player[] _players;

        static Player() {
            var config = Config.Instance;
            _players = new Player[config.MaxPlayers];
            for (var i = 0; i < _players.Length; i++) {
                _players[i] = new Player(i);
            }
        }

        /// <summary>
        /// Retrieves a Player object given it's corresponding number.
        /// Note: Player numbers are 0 indexed. What appears as Player 1 is actually player number 0 internally.
        /// </summary>
        /// <param name="playerNumber">the Player's number identifier</param>
        /// <exception cref="ArgumentException">thrown if <param name="playerNumber"> is less than 0 or greater 
        ///     than or equal to the maximum number of players</param></exception>
        /// <returns>the corresponding Player object</returns>
        public static Player GetPlayerData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= _players.Length)
                throw new ArgumentException("playerNumber");
            return _players[playerNumber];
        }

        /// <summary>
        /// Gets an enumeration of all Player.
        /// Note this includes all inactive players as well.
        /// </summary>
        public static IEnumerable<Player> AllPlayers {
            get {
                foreach (var player in _players)
                    yield return player;
            }
        }

        /// <summary>
        /// Gets an enumeration of all active AllPlayers.
        /// To get an enumeration of all AllPlayers, active or not, see <see cref="AllPlayers"/>
        /// </summary>
        public static IEnumerable<Player> ActivePlayers {
            get { return _players.Where(player => player.Type != Player.PlayerType.None); }
        }

        /// <summary>
        /// Gets the maximum number of players allowed in one match.
        /// </summary>
        public static int MaxPlayers {
            get { return _players.Length; }
        }

        /// <summary>
        /// Gets the count of currently active AllPlayers.
        /// </summary>
        public static int ActivePlayerCount {
            get { return ActivePlayers.Count(); }
        }

        public enum PlayerType {

            None = 0,
            CPU = 1,
            HumanPlayer = 2

        };

        private readonly int _playerNumber;

        public int PlayerNumber {
            get { return _playerNumber; }
        }

        public event Action OnChanged;

        private CharacterData _character;
        private int _level;
        private int _pallete;

        public CharacterData Character {
            get { return _character; }
            set {
                bool changed = _character == value;
                _character = value;
                if (changed && OnChanged != null)
                    OnChanged();
            }
        }

        public int CPULevel {
            get { return _level; }
            set {
                bool changed = _level == value;
                _level = value;
                if (changed && OnChanged != null)
                    OnChanged();
            }
        }

        public int Pallete {
            get { return _pallete; }
            set {
                bool changed = _pallete == value;
                _pallete = value;
                if (changed && OnChanged != null)
                    OnChanged();
            }
        }

        public PlayerType Type { get; set; }

        public InputDevice Controller {
            get {
                if (PlayerNumber < 0 || PlayerNumber >= InputManager.Devices.Count)
                    return null;
                return InputManager.Devices[PlayerNumber];
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
                //if (Type == PlayerType.None)
                //    return Color.clear;
                Config config = Config.Instance;
                if (Type == PlayerType.CPU)
                    return config.CPUColor;
                return config.GetPlayerColor(PlayerNumber);
            }
        }

        internal Player(int number) {
            _playerNumber = number;
            CPULevel = 3;
        }

        internal Character Spawn(Transform transform = null) {
            if(transform == null)
                throw new ArgumentNullException("transform");
            return Spawn(transform.position, transform.rotation);
        }

        internal Character Spawn(Vector3 pos, Quaternion rot) {
            GameObject prefab = Character.Prefab.Load();
            if (prefab == null)
                return null;
            SpawnedCharacter = prefab.Duplicate(pos, rot).GetComponent<Character>();
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
