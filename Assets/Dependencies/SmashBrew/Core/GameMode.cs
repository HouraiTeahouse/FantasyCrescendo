// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> An abstract class for controlling the global status of the game while under a certain game mode. </summary>
    public abstract class GameMode {
        static GameMode _current;

        /// <summary> The current game mode. </summary>
        public static GameMode Current {
            get {
                return _current ?? (_current = Config.GameModes.StandardVersus);
            }
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
        public abstract ReadOnlyCollection<CharacterData> ExcludedCharacters {
            get; }

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
            get {
                return new ReadOnlyCollection<CharacterData>(_excludedCharacters);
            }
        }

        public override ReadOnlyCollection<SceneData> ExcludedStages {
            get { return new ReadOnlyCollection<SceneData>(_excludedStages); }
        }
    }

    public abstract class MultiMatchGameMode : GameMode {
    }
}