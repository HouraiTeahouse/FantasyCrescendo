using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerSet : IEnumerable<Player> {

        Player[] _players;

        public Player Get(int id) {
            if(!Check.Range(id, _players))
                throw new ArgumentOutOfRangeException();
            Assert.IsTrue(id < GameMode.Current.MaxPlayers);
            return _players[id];
        }

        public IEnumerator<Player> GetEnumerator() { return _players.Cast<Player>().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count {
            get { return _players.Length; }
        }

        public PlayerSet() {
            // Note: These objects are not intended to be destroyed and thus do not unresgister these event handlers
            // If there is a need to remove or replace them, this will need to be changed.
            GameMode.OnRegister += mode => RebuildPlayerArray();
            GameMode.OnChangeGameMode += mode => RebuildPlayerArray();
            _players = new Player[0];
            RebuildPlayerArray();
        }

        void RebuildPlayerArray() {
            var maxPlayers= !GameMode.All.Any() ? 0 : GameMode.All.Max(mode => mode.MaxPlayers);
            if (maxPlayers <= Count)
                return;
            var temp = new Player[maxPlayers];
            if(_players != null)
                Array.Copy(_players, temp , _players.Length);
            for (var i = 0; i < temp.Length; i++) {
                if(temp[i] == null)
                    temp[i] = new Player(i);
            }
            _players = temp;
        }

        public void ResetAll() {
            var blankSelection = new PlayerSelection();
            foreach (Player player in _players) {
                if (player == null)
                    continue;
                player.Selection = blankSelection;
                player.Type = PlayerType.None;
            }
        }

    }

    public class PlayerManager : MonoBehaviour {

        Player[] _localPlayers;
        Player[] _matchPlayers;

        public static PlayerManager Instance { get; private set; }

        public PlayerSet LocalPlayers { get; private set; }
        public PlayerSet MatchPlayers { get; private set; }

        public int MaxPlayers {
            get {
                Assert.IsTrue(LocalPlayers.Count == MatchPlayers.Count);
                return LocalPlayers.Count;
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
            Config.Load();
            LocalPlayers = new PlayerSet();
            MatchPlayers = new PlayerSet();
            //TODO(james7132): Have this filled out externally.
            var player = LocalPlayers.Get(0);
            player.Type = PlayerType.HumanPlayer;
            player.Selection = new PlayerSelection {
                Character = DataManager.Characters.FirstOrDefault(c => c.IsSelectable)
            };
        }

    }

}
