using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerType {

        public static readonly PlayerType[] Types;

        public static readonly PlayerType None = new PlayerType {
            Name = "None",
            ShortName = "",
            IsActive = false,
            IsCPU = false,
            Color = UnityEngine.Color.clear
        };

        public static readonly PlayerType HumanPlayer = new PlayerType {
            Name = "Player {0}",
            ShortName = "P{0}",
            IsActive = true,
            IsCPU = false,
            Color = null
        };

        public static readonly PlayerType CPU = new PlayerType {
            Name = "CPU",
            ShortName = "CPU",
            IsActive = true,
            IsCPU = true,
            Color = UnityEngine.Color.gray
        };

        static PlayerType() {
            Types = new[] {None, HumanPlayer, CPU};
            for (var i = 0; i < Types.Length - 1; i++)
                Types[i].Next = Types[i + 1];
            Types[Types.Length - 1].Next = Types[0];
        }

        // Private constructor
        PlayerType() { }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsCPU { get; private set; }
        public Color? Color { get; private set; }
        public PlayerType Next { get; private set; }

        public override string ToString() { return Name; }

    }

    [Serializable]
    public class PlayerSelection {

        [SerializeField]
        CharacterData _character;

        [SerializeField]
        int _pallete;

        /// <summary> The Player's selected Character. If null, a random Character will be spawned when the match starts. </summary>
        public CharacterData Character {
            get { return _character; }
            set {
                if (_character == value)
                    return;
                _character = value;
                Changed.SafeInvoke();
            }
        }

        /// <summary> The Player's selected </summary>
        public int Pallete {
            get { return _pallete; }
            set {
                if (_pallete == value)
                    return;
                if (_character) {
                    int count = _character.PalleteCount;
                    if (count > 0)
                        // This is
                        _pallete = value - count * Mathf.FloorToInt(value / (float) count);
                }
                else {
                    _pallete = value;
                }
                Changed.SafeInvoke();
            }
        }

        public event Action Changed;

        public void Copy(PlayerSelection selection) {
            if (this == selection)
                return;
            Argument.NotNull(selection);
            Pallete = selection.Pallete;
            Character = selection.Character;
        }

        public static bool operator ==(PlayerSelection s1, PlayerSelection s2) {
            bool n1 = ReferenceEquals(s1, null);
            bool n2 = ReferenceEquals(s2, null);
            return n1 ^ n2 || (!n1 && s1.Character == s2.Character && s1.Pallete == s2.Pallete);
        }

        public static bool operator !=(PlayerSelection s1, PlayerSelection s2) { return !(s1 == s2); }

        public override bool Equals(object obj) { return this == obj as PlayerSelection; }

        public override int GetHashCode() {
            unchecked {
                return ((_character != null ? _character.GetHashCode() : 0) * 397) ^ _pallete;
            }
        }

    }

    public sealed class Player {

        //TODO: Make this more dynamic
        static readonly Player[] _players;
        static readonly ReadOnlyCollection<Player> _playerCollection;
        int _level;
        PlayerType _type;
        readonly PlayerSelection _selection;

        static Player() {
            _players = new Player[GameMode.Current.MaxPlayers];
            for (var i = 0; i < _players.Length; i++)
                _players[i] = new Player(i);
            _playerCollection = new ReadOnlyCollection<Player>(_players);
        }

        internal Player(int number) {
            ID = number;
            Type = PlayerType.Types[0];
            _selection = new PlayerSelection();
            Selection.Changed += () => Changed.SafeInvoke();
            Level = 3;
        }

        public PlayerSelection Selection {
            get { return _selection; }
            set { _selection.Copy(value); }
        }

        /// <summary> Gets an enumeration of all Player. Note this includes all inactive players as well. </summary>
        public static ReadOnlyCollection<Player> AllPlayers {
            get { return _playerCollection; }
        }

        /// <summary> Gets an enumeration of all active AllPlayers. To get an enumeration of all AllPlayers, active or not, see
        /// <see cref="AllPlayers" />
        /// </summary>
        public static IEnumerable<Player> ActivePlayers {
            get { return _players.Where(player => player.Type.IsActive); }
        }

        /// <summary> Gets the maximum number of players allowed in one match. </summary>
        public static int MaxPlayers {
            get { return _players.Length; }
        }

        /// <summary> Gets the count of currently active Players. </summary>
        public static int ActiveCount {
            get { return ActivePlayers.Count(); }
        }

        public int ID { get; private set; }

        /// <summary> Gets the CharacterData specifying the actual character spawned for the Player. May be different from
        /// <see cref="Selection" />'s selected character, particularly if the Character was randomly selected. </summary>
        public CharacterData SpawnedCharacter { get; private set; }

        /// <summary> The AI level </summary>
        public int Level {
            get { return _level; }
            set {
                if (_level == value)
                    return;
                _level = value;
                Changed.SafeInvoke();
            }
        }

        public PlayerType Type {
            get { return _type; }
            set { _type = Argument.NotNull(value); }
        }

        public InputDevice Controller {
            get { return Check.Range(ID, HInput.Devices.Count) ? HInput.Devices[ID] : null; }
        }

        /// <summary> The actual spawned GameObject of the Player. </summary>
        public Character PlayerObject { get; private set; }

        // The represnetative color of this player. Used in UI.
        public Color Color {
            get { return Type.Color ?? Config.Player.GetColor(ID); }
        }

        /// <summary> Retrieves a Player object given it's corresponding number. Note: Player numbers are 0 indexed. What appears
        /// as Player 1 is actually player number 0 internally. </summary>
        /// <param name="id"> the Player's number identifier </param>
        /// <exception cref="ArgumentException">
        ///     <param name="id"> is less than 0 or greater than or equal to the maximum number of players </param>
        /// </exception>
        /// <returns> the corresponding Player object </returns>
        public static Player Get(int id) {
            Argument.Check("playerNumber", Check.Range(id, _players));
            return _players[id];
        }

        public event Action Changed;

        public static Player Get(GameObject go) {
            if (!go)
                return null;
            var controller = go.GetComponentInParent<PlayerController>();
            return !controller ? null : controller.PlayerData;
        }

        public static Player Get(Component comp) { return !comp ? null : Get(comp.gameObject); }

        public static bool IsPlayer(GameObject go) { return Get(go) != null; }

        public static bool IsPlayer(Component comp) { return Get(comp) != null; }

        public void CycleType() {
            Type = Type.Next;
            Changed.SafeInvoke();
        }

        internal Character Spawn(Transform transform, bool direction) {
            return Spawn(Argument.NotNull(transform).position, direction);
        }

        internal Character Spawn(Vector3 pos, bool direction) {
            SpawnedCharacter = Selection.Character;
            int color = Selection.Pallete;

            // If the character is null, randomly select a character and pallete
            if (SpawnedCharacter == null) {
                SpawnedCharacter = DataManager.Instance.Characters.Random();
                color = Random.Range(0, SpawnedCharacter.PalleteCount);
            }

            //TODO: Make the loading of characters asynchronous 
            GameObject prefab = SpawnedCharacter.Prefab.Load();
            if (prefab == null)
                return null;
            PlayerObject = prefab.Duplicate(pos).GetComponent<Character>();
            PlayerObject.Direction = direction;
            var controller = PlayerObject.GetComponentInChildren<PlayerController>();
            var materialSwap = PlayerObject.GetComponentInChildren<MaterialSwap>();
            if (controller)
                controller.PlayerData = this;
            if (materialSwap)
                materialSwap.Pallete = color;
            return PlayerObject;
        }

        public string GetName(bool shortName = false) {
            return string.Format(shortName ? Type.ShortName : Type.Name, ID + 1);
        }

        public override string ToString() { return GetName(); }

        public override bool Equals(object obj) {
            var player = obj as Player;
            if (player != null)
                return this == player;
            return false;
        }

        public override int GetHashCode() { return ID; }

        public static bool operator ==(Player p1, Player p2) {
            bool nullCheck1 = ReferenceEquals(p1, null);
            bool nullCheck2 = ReferenceEquals(p2, null);
            return !(nullCheck1 ^ nullCheck2) && (nullCheck1 || p1.ID == p2.ID);
        }

        public static bool operator !=(Player p1, Player p2) { return !(p1 == p2); }

    }

}