using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

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

        public event Action Changed;

        public bool Copy(PlayerSelection selection) {
            if (this == selection)
                return false;
            Argument.NotNull(selection);
            Pallete = selection.Pallete;
            Character = selection.Character;
            return true;
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

}
