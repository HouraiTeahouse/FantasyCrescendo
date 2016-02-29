using System;
using UnityEngine;
using System.Collections.ObjectModel;

namespace HouraiTeahouse.SmashBrew {

    public abstract class GameMode {

        public static GameMode Current { get; set; }

        public abstract int MaxPlayers { get; }
        public abstract int MinPlayers { get; }
        public abstract bool CPUsAllowed { get; }
        public abstract ReadOnlyCollection<CharacterData> ExcludedCharacters { get; }
        public abstract ReadOnlyCollection<SceneData> ExcludedStages { get; }

    }

    [Serializable]
    internal sealed class SerializedGameMode : GameMode {

        [SerializeField]
        private int _minimumPlayers = 1;

        [SerializeField]
        private int _maximumPlayers = 4;

        [SerializeField]
        private bool _cpusAllowed = true;

        [SerializeField]
        private CharacterData[] _excludedCharacters;

        [SerializeField]
        private SceneData[] _excludedStages;

        public override int MaxPlayers {
            get { return _maximumPlayers; }
        }

        public override int MinPlayers {
            get { return _minimumPlayers; }
        }

        public override bool CPUsAllowed {
            get { return _cpusAllowed; }
        }

        public override ReadOnlyCollection<CharacterData> ExcludedCharacters {
            get { return new ReadOnlyCollection<CharacterData>(_excludedCharacters); }
        }

        public override ReadOnlyCollection<SceneData> ExcludedStages {
            get { return new ReadOnlyCollection<SceneData>(_excludedStages); }
        }
    }
}
