using System;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.Networking;

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
            for (short i = 0; i < Types.Length - 1; i++) {
                Types[i].Next = Types[i + 1];
                Types[i].ID = i;
            }
            Types[Types.Length - 1].Next = Types[0];
        }

        // Private constructor
        PlayerType() { }
        public short ID { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsCPU { get; private set; }
        public Color? Color { get; private set; }
        public PlayerType Next { get; private set; }

        public override string ToString() { return Name; }

    }


    public class PlayerSelectionMessage : MessageBase {

        public uint CharacterID;
        public int Pallete;
        public int CPULevel;

    }

    [Serializable]
    public class PlayerSelection {

        [SerializeField]
        CharacterData _character;

        [SerializeField]
        int _pallete;

        [SerializeField]
        int _cpuLevel = 0;

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
                        _pallete = value - count * Mathf.FloorToInt(value / (float) count);
                }
                else {
                    _pallete = value;
                }
                Changed.SafeInvoke();
            }
        }

        public int CPULevel {
            get { return _cpuLevel; }
            set {
                if (_cpuLevel == value)
                    return;
                _cpuLevel = value;
                Changed.SafeInvoke();
            }
        }

        public bool IsCPU {
            get { return CPULevel > 0; }
        }

        public PlayerSelectionMessage ToMessage() {
            return new PlayerSelectionMessage {
                CharacterID = Character != null ? Character.Id : 0,
                Pallete = Pallete,
                CPULevel = CPULevel,
            };
        }

        public static PlayerSelection FromMessage(PlayerSelectionMessage message) {
            return new PlayerSelection {
                Character = DataManager.Instance.GetCharacter(message.CharacterID),
                CPULevel = message.CPULevel,
                Pallete = message.Pallete
            };
        }

        public event Action Changed;

        public void Copy(PlayerSelection selection) {
            if (this == selection)
                return;
            Argument.NotNull(selection);
            Pallete = selection.Pallete;
            Character = selection.Character;
        }

        public override string ToString() {
            if (Character == null)
                return "None";
            return "{0}:{1}".With(Character.name, Pallete);
        }

        public static bool operator ==(PlayerSelection s1, PlayerSelection s2) {
            bool n1 = ReferenceEquals(s1, null);
            bool n2 = ReferenceEquals(s2, null);
            return (n1 && n2) || (n1 == n2 && s1.Character == s2.Character && s1.Pallete == s2.Pallete);
        }

        public static bool operator !=(PlayerSelection s1, PlayerSelection s2) { return !(s1 == s2); }

        public override bool Equals(object obj) { return this == obj as PlayerSelection; }

        public override int GetHashCode() {
            unchecked {
                return ((_character != null ? _character.GetHashCode() : 0) * 397) ^ _pallete;
            }
        }

    }

    public class Player : MessageBase {

        PlayerType _type;

        [SerializeField]
        PlayerSelection _selection;

        [SerializeField]
        short _typeId;

        internal Player(int number) {
            ID = number;
            Type = PlayerType.Types[0];
            _selection = new PlayerSelection();
            Selection.Changed += () => Changed.SafeInvoke();
        }

        public override void Deserialize(NetworkReader reader) {
            base.Deserialize(reader);
            _type = PlayerType.Types[_typeId];
        }

        public override void Serialize(NetworkWriter writer) {
            _typeId = _type.ID;
            base.Serialize(writer);
        }

        public PlayerSelection Selection {
            get { return _selection; }
            set {
                _selection.Copy(value);
                Log.Info("Set Player {0}'s selection to {1}".With(ID, _selection));
            }
        }

        public int ID { get; private set; }

        /// <summary> Gets the CharacterData specifying the actual character spawned for the Player. May be different from
        /// <see cref="Selection" />'s selected character, particularly if the Character was randomly selected. </summary>
        public CharacterData SpawnedCharacter { get; private set; }

        public PlayerType Type {
            get { return _type; }
            set { _type = Argument.NotNull(value); }
        }

        public InputDevice Controller {
            get { return Check.Range(ID, HInput.Devices.Count) ? HInput.Devices[ID] : null; }
        }

        // The represnetative color of this player. Used in UI.
        public Color Color {
            get { return Type.Color ?? Config.Player.GetColor(ID); }
        }

        public event Action Changed;

        public void CycleType() {
            Type = Type.Next;
            Changed.SafeInvoke();
        }

        //internal Character Spawn(Transform transform, bool direction) {
        //    return Spawn(Argument.NotNull(transform).position, direction);
        //}

        //internal Character Spawn(Vector3 pos, bool direction) {
        //    SpawnedCharacter = Selection.Character;
        //    int color = Selection.Pallete;
        //    // If the character is null, randomly select a character and pallete
        //    if (SpawnedCharacter == null) {
        //        SpawnedCharacter = DataManager.Instance.Characters.Random();
        //        color = Random.Range(0, SpawnedCharacter.PalleteCount);
        //    }
        //    //TODO: Make the loading of characters asynchronous 
        //    GameObject prefab = SpawnedCharacter.Prefab.Load();
        //    if (prefab == null)
        //        return null;
        //    PlayerObject = prefab.Duplicate(pos).GetComponent<Character>();
        //    //PlayerObject.Direction = direction;
        //    var controller = PlayerObject.GetComponentInChildren<PlayerController>();
        //    var materialSwap = PlayerObject.GetComponentInChildren<ColorState>();
        //    if (controller)
        //        controller.PlayerData = this;
        //    if (materialSwap)
        //        materialSwap.Pallete = color;
        //    return PlayerObject;
        //}

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
            bool n1 = ReferenceEquals(p1, null);
            bool n2 = ReferenceEquals(p2, null);
            return (n1 && n2) || (n1 == n2 && p1.ID == p2.ID);
        }

        public static bool operator !=(Player p1, Player p2) { return !(p1 == p2); }

    }

}
