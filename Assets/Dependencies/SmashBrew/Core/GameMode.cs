using System;
using UnityEngine;
using System.Collections.ObjectModel;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// An abstract class for controlling the global status of the game while under a certain game mode.
    /// </summary>
    public abstract class GameMode {
        private static GameMode _current;

        /// <summary>
        /// The current game mode.
        /// </summary>
        public static GameMode Current {
            get { return _current ?? (_current = Config.GameModes.StandardVersus); }
            set { _current = value ?? Config.GameModes.StandardVersus; }
        }

        /// <summary>
        /// The maximum number of chosen players in a match under this game mode.
        /// This does not affect the number of game-inserted number of players in the match.
        /// </summary>
        public abstract int MaxPlayers { get; }

        /// <summary>
        /// The minimum number of chosen players in a match to start playing the game mode.
        /// </summary>
        public abstract int MinPlayers { get; }

        /// <summary>
        /// Whether choosing CPU characters is OK for the game mode
        /// </summary>
        public abstract bool CPUsAllowed { get; }

        /// <summary>
        /// All of the characters that cannot be selected for this  
        /// </summary>
        public abstract ReadOnlyCollection<CharacterData> ExcludedCharacters { get; }

        /// <summary>
        /// All of the stages that cannot be selected for the game mode
        /// </summary>
        public abstract ReadOnlyCollection<SceneData> ExcludedStages { get; }
    }

    [Serializable]
    public sealed class SerializedGameMode : GameMode {
        [SerializeField] private int _minimumPlayers = 1;

        [SerializeField] private int _maximumPlayers = 4;

        [SerializeField] private bool _cpusAllowed = true;

        [SerializeField] private CharacterData[] _excludedCharacters;

        [SerializeField] private SceneData[] _excludedStages;

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

    public abstract class MultiMatchGameMode : GameMode {
    }
}
