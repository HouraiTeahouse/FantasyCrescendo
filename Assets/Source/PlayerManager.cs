using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerManager : NetworkBehaviour {

        int _maxPlayers = -1;
        Player[] _localPlayers;
        Player[] _matchPlayers;

        public static PlayerManager Instance { get; private set; }

        public IEnumerable<Player> LocalPlayers {
            get { return _localPlayers.Select(x => x); }
        }

        public IEnumerable<Player> MatchPlayers {
            get { return _matchPlayers.Select(x => x); }
        }

        public int MaxPlayers {
            get { return _maxPlayers; }
        }

        public Player GetLocalPlayer(int id) {
            if(!Check.Range(id, 0, GameMode.Current.MaxPlayers))
                throw new ArgumentOutOfRangeException();
            if(id >= _maxPlayers)
                RebuildPlayerArray();
            return _localPlayers[id];
        }

        public Player GetMatchPlayer(int id) {
            if(!Check.Range(id, 0, GameMode.Current.MaxPlayers))
                throw new ArgumentOutOfRangeException();
            if(id >= _maxPlayers)
                RebuildPlayerArray();
            return _matchPlayers[id];
        }

        public void ResetMatchPlayers() {
            var blankSelection = new PlayerSelection();
            foreach (Player player in MatchPlayers) {
                if (player == null)
                    continue;
                player.Selection = blankSelection;
                player.Type = PlayerType.None;
            }
        }

        [SerializeField]
        PlayerSelection[] testCharacters;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            Instance = this;
            Config.Load();
            RebuildPlayerArray();
            GameMode.OnRegister += mode => RebuildPlayerArray();
        }

        void RebuildPlayerArray() {
            var check = !GameMode.All.Any() ? 0 : GameMode.All.Max(mode => mode.MaxPlayers);
            if (check <= _maxPlayers)
                return;
            _maxPlayers = check;
            _localPlayers = CopyPlayerSet(_localPlayers, _maxPlayers);
            _matchPlayers = CopyPlayerSet(_matchPlayers, _maxPlayers);
            for (var i = 0; i < _maxPlayers; i++) {
                if(_localPlayers[i] == null)
                    _localPlayers[i] = new Player(i);
                if(_matchPlayers[i] == null)
                    _matchPlayers[i] = new Player(i);
            }
//#if UNITY_EDITOR
            var index = 0;
            foreach (Player player in LocalPlayers) {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.Selection = testCharacters[index];
                player.Type = player.Selection.Character ? PlayerType.HumanPlayer : PlayerType.None;
                index++;
            }
//#endif
        }

        Player[] CopyPlayerSet(Player[] set, int size) {
            if (set != null && size <= set.Length)
                return set;
            var temp = new Player[size];
            if(set != null)
                Array.Copy(set, temp , set.Length);
            return temp;
        }

        public PlayerSelection GetSelection(int playerNum) {
            if(testCharacters.Length <= 0)
                throw new ArgumentException();
            return testCharacters[playerNum % testCharacters.Length];
        }


    }

}
