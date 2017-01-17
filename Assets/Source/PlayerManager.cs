using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerManager : NetworkBehaviour {

        int _maxPlayers;
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
            if(Check.Range(id, 0, GameMode.Current.MaxPlayers))
                throw new ArgumentOutOfRangeException();
            return _localPlayers[id];
        }

        public Player GetMatchPlayer(int id) {
            if(Check.Range(id, 0, GameMode.Current.MaxPlayers))
                throw new ArgumentOutOfRangeException();
            return _matchPlayers[id];
        }

        [SerializeField]
        PlayerSelection[] testCharacters;

        void Awake() {
            Instance = this;
            _maxPlayers = GameMode.All.Max(mode => mode.MaxPlayers);
            _localPlayers = new Player[_maxPlayers];
            _matchPlayers = new Player[_maxPlayers];
            for (var i = 0; i < _maxPlayers; i++) {
                _localPlayers[i] = new Player(i);
                _matchPlayers[i] = new Player(i);
            }
        }

        public PlayerSelection GetSelection(int playerNum) {
            if(testCharacters.Length <= 0)
                throw new ArgumentException();
            return testCharacters[playerNum % testCharacters.Length];
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        //void Awake() {
        //    var index = 0;
        //    foreach (Player player in Player.AllPlayers) {
        //        if (index >= testCharacters.Length)
        //            break;
        //        if (player == null || testCharacters[index] == null)
        //            continue;
        //        player.Selection = testCharacters[index];
        //        player.Type = player.Selection.Character ? PlayerType.HumanPlayer : PlayerType.None;
        //        index++;
        //    }
        //}

    }

}
