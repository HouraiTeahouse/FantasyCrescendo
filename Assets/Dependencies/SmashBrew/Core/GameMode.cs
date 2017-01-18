using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> An abstract class for controlling the global status of the game while under a certain game mode. </summary>
    public abstract class GameMode {

        static GameMode _current;

        static readonly List<GameMode> _gameModes;

        /// <summary>
        /// Event called when a new game mode is registered
        /// </summary>
        public static event Action<GameMode> OnRegister;

        static GameMode() { _gameModes = new List<GameMode>(); }

        public static IEnumerable<GameMode> All {
            get { return _gameModes.Select(x => x); }
        }

        public static void Register(GameMode mode) {
            Argument.NotNull(mode);
            if(!_gameModes.Contains(mode)) {
                _gameModes.Add(mode);
                OnRegister.SafeInvoke(mode);
            }
        }

        /// <summary> The current game mode. </summary>
        public static GameMode Current {
            get { return _current ?? (_current = Config.GameModes.StandardVersus); }
            set { _current = value ?? Config.GameModes.StandardVersus; }
        }

        /// <summary> The maximum number of chosen players in a match under this game mode. This does not affect the number of
        /// game-inserted number of players in the match. </summary>
        public abstract int MaxPlayers { get; }

        /// <summary> The minimum number of chosen players in a match to start playing the game mode. </summary>
        public abstract int MinPlayers { get; }

        /// <summary> Whether choosing CPU characters is OK for the game mode </summary>
        public abstract bool CPUsAllowed { get; }

        /// <summary> All of the characters that cannot be selected for this </summary>
        public abstract ReadOnlyCollection<CharacterData> ExcludedCharacters { get; }

        /// <summary> All of the stages that cannot be selected for the game mode </summary>
        public abstract ReadOnlyCollection<SceneData> ExcludedStages { get; }

    }

    [Serializable]
    public sealed class SerializedGameMode : GameMode {

        [SerializeField]
        bool _cpusAllowed = true;

        [SerializeField]
        CharacterData[] _excludedCharacters;

        [SerializeField]
        SceneData[] _excludedStages;

        [SerializeField]
        int _maximumPlayers = 4;

        [SerializeField]
        int _minimumPlayers = 1;

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
